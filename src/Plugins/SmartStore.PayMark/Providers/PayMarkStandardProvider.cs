using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using SmartStore.Core.Domain.Common;
using SmartStore.Core.Domain.Orders;
using SmartStore.Core.Domain.Payments;
using SmartStore.Core.Domain.Shipping;
using SmartStore.Core.Logging;
using SmartStore.Core.Plugins;
using SmartStore.PayMark.Controllers;
using SmartStore.PayMark.Settings;
using SmartStore.Services;
using SmartStore.Services.Localization;
using SmartStore.Services.Orders;
using SmartStore.Services.Payments;

namespace SmartStore.PayMark
{
	[SystemName("Payments.PayMarkStandard")]
	[FriendlyName("PayMark Standard")]
	[DisplayOrder(1)]
	public partial class PayMarkStandardProvider : PaymentPluginBase, IConfigurable
	{
		private readonly IOrderTotalCalculationService _orderTotalCalculationService;
		private readonly ICommonServices _services;
		private readonly ILogger _logger;

		public PayMarkStandardProvider(
			IOrderTotalCalculationService orderTotalCalculationService,
			ICommonServices services,
			ILogger logger)
		{
			_orderTotalCalculationService = orderTotalCalculationService;
			_services = services;
			_logger = logger;
		}

		public static string SystemName
		{
			get { return "Payments.PayMarkStandard"; }
		}

		public override PaymentMethodType PaymentMethodType
		{
			get { return PaymentMethodType.Redirection; }
		}

		/// <summary>
		/// Process a payment
		/// </summary>
		/// <param name="processPaymentRequest">Payment info required for an order processing</param>
		/// <returns>Process payment result</returns>
		public override ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
		{
			var result = new ProcessPaymentResult
			{
				NewPaymentStatus = PaymentStatus.Pending
			};

			return result;
		}

		/// <summary>
		/// Post process payment (used by payment gateways that require redirecting to a third-party URL)
		/// </summary>
		/// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
		public override void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
		{
			if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
				return;

			var store = _services.StoreService.GetStoreById(postProcessPaymentRequest.Order.StoreId);
			var settings =
				_services.Settings.LoadSetting<PayMarkStandardPaymentSettings>(postProcessPaymentRequest.Order.StoreId);

			string orderNumber = postProcessPaymentRequest.Order.GetOrderNumber();
			var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);


			var returnUrl = _services.WebHelper.GetStoreLocation(store.SslEnabled) +
			                "Plugins/SmartStore.PayMark/PayMarkStandard/PDTHandler";
			Guard.NotNull(settings.AccountId, nameof(settings.AccountId));
			Guard.NotNull(settings.ApiAccountPassword, nameof(settings.ApiAccountPassword));
			Guard.NotNull(settings.ClientId, nameof(settings.ClientId));

			var queryParams = new Dictionary<string, string>
			{
				{PaymentRequestParams.CMD, settings.PaymentCommand},
				{PaymentRequestParams.AMOUNT, orderTotal.ToString("0.00", CultureInfo.InvariantCulture)},
				{PaymentRequestParams.PASSWORD, settings.ApiAccountPassword},
				{PaymentRequestParams.USERNAME, settings.ClientId},
				{PaymentRequestParams.ACCOUNT_ID, settings.AccountId},
				{PaymentRequestParams.REFERENCE, postProcessPaymentRequest.Order.OrderGuid.ToString()},
				{PaymentRequestParams.RETURN_URL, returnUrl},
				{PaymentRequestParams.PARTICULAR, orderNumber},
				{PaymentRequestParams.STORE_PAYMENT_TOKEN, "2"},
				{PaymentRequestParams.DISPLAY_CUSTOMER_EMAIL, "1"},
			};
			var client = new HttpClient();
			var response = client.PostAsync(QueryHelpers.AddQueryString(settings.PayMarkUrl, queryParams),
				new FormUrlEncodedContent(queryParams)).Result;
			var data = response.Content.ReadAsStringAsync().Result;

			var element = XElement.Parse(data);
			Uri redirectUri = new Uri(element.Value);
			postProcessPaymentRequest.RedirectUrl = redirectUri.ToString();

		}

		/// <summary>
		/// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Result</returns>
		public override bool CanRePostProcessPayment(Order order)
		{
			if (order == null)
				throw new ArgumentNullException("order");

			if (order.PaymentStatus == PaymentStatus.Pending && (DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds > 5)
			{
				return true;
			}

			return true;
		}

		public override Type GetControllerType()
		{
			return typeof(PayMarkStandardController);
		}

		public override decimal GetAdditionalHandlingFee(IList<OrganizedShoppingCartItem> cart)
		{
			var result = decimal.Zero;
			try
			{
				var settings =
					_services.Settings.LoadSetting<PayMarkStandardPaymentSettings>(_services.StoreContext.CurrentStore
						.Id);

				result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart, settings.AdditionalFee,
					settings.AdditionalFeePercentage);
			}
			catch (Exception)
			{
			}

			return result;
		}

		/// <summary>
		/// Gets PDT details
		/// </summary>
		/// <param name="tx">TX</param>
		/// <param name="values">Values</param>
		/// <param name="response">Response</param>
		/// <returns>Result</returns>
		public bool GetPDTDetails(string tx, PayMarkStandardPaymentSettings settings,
			out TransactionResult values, out string response)
		{
			Guard.NotNull(settings.PayMarkTransactionUrl, nameof(settings.PayMarkTransactionUrl));
			Guard.NotNull(settings.SecretHashKey, nameof(settings.SecretHashKey));
			var request = new HttpClient();
			request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
				System.Convert.ToBase64String(
					Encoding.UTF8.GetBytes($"{settings.ClientId}:{settings.ApiAccountPassword}")));
			var result = request.GetAsync($"{settings.PayMarkTransactionUrl}{tx}").Result;

			response = result.Content.ReadAsStringAsync().Result;

			values = JsonConvert.DeserializeObject<TransactionResult>(response);

			return values.status.ToLower().Equals("successful");
		}

		/// <summary>
		/// Splits the difference of two value into a portion value (for each item) and a rest value
		/// </summary>
		/// <param name="difference">The difference value</param>
		/// <param name="numberOfLines">Number of lines\items to split the difference</param>
		/// <param name="portion">Portion value</param>
		/// <param name="rest">Rest value</param>
		private void SplitDifference(decimal difference, int numberOfLines, out decimal portion, out decimal rest)
		{
			portion = rest = decimal.Zero;

			if (numberOfLines == 0)
				numberOfLines = 1;

			int intDifference = (int) (difference * 100);
			int intPortion = (int) Math.Truncate((double) intDifference / (double) numberOfLines);
			int intRest = intDifference % numberOfLines;

			portion = Math.Round(((decimal) intPortion) / 100, 2);
			rest = Math.Round(((decimal) intRest) / 100, 2);

			Debug.Assert(difference == ((numberOfLines * portion) + rest));
		}

		/// <summary>
		/// Get all PayMark line items
		/// </summary>
		/// <param name="postProcessPaymentRequest">Post process paymenmt request object</param>
		/// <param name="checkoutAttributeValues">List with checkout attribute values</param>
		/// <param name="cartTotal">Receives the calculated cart total amount</param>
		/// <returns>All items for PayMark Standard API</returns>
		public List<PayMarkLineItem> GetLineItems(PostProcessPaymentRequest postProcessPaymentRequest,
			out decimal cartTotal)
		{
			cartTotal = decimal.Zero;

			var order = postProcessPaymentRequest.Order;
			var lst = new List<PayMarkLineItem>();

			// Order items... checkout attributes are included in order total
			foreach (var orderItem in order.OrderItems)
			{
				var item = new PayMarkLineItem
				{
					Type = PayMarkItemType.CartItem,
					Name = orderItem.Product.GetLocalized(x => x.Name),
					Quantity = orderItem.Quantity,
					Amount = orderItem.UnitPriceExclTax
				};
				lst.Add(item);

				cartTotal += orderItem.PriceExclTax;
			}

			// Shipping
			if (order.OrderShippingExclTax > decimal.Zero)
			{
				var item = new PayMarkLineItem
				{
					Type = PayMarkItemType.Shipping,
					Name = T("Plugins.Payments.PayMarkStandard.ShippingFee").Text,
					Quantity = 1,
					Amount = order.OrderShippingExclTax
				};
				lst.Add(item);

				cartTotal += order.OrderShippingExclTax;
			}

			// Payment fee
			if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
			{
				var item = new PayMarkLineItem
				{
					Type = PayMarkItemType.PaymentFee,
					Name = T("Plugins.Payments.PayMark.PaymentMethodFee").Text,
					Quantity = 1,
					Amount = order.PaymentMethodAdditionalFeeExclTax
				};
				lst.Add(item);

				cartTotal += order.PaymentMethodAdditionalFeeExclTax;
			}

			// Tax
			if (order.OrderTax > decimal.Zero)
			{
				var item = new PayMarkLineItem
				{
					Type = PayMarkItemType.Tax,
					Name = T("Plugins.Payments.PayMarkStandard.SalesTax").Text,
					Quantity = 1,
					Amount = order.OrderTax
				};
				lst.Add(item);

				cartTotal += order.OrderTax;
			}

			return lst;
		}

		/// <summary>
		/// Gets a route for provider configuration
		/// </summary>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name</param>
		/// <param name="routeValues">Route values</param>
		public override void GetConfigurationRoute(out string actionName, out string controllerName,
			out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "PayMarkStandard";
			routeValues = new RouteValueDictionary() {{"area", "SmartStore.PayMark"}};
		}

		/// <summary>
		/// Gets a route for payment info
		/// </summary>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name</param>
		/// <param name="routeValues">Route values</param>
		public override void GetPaymentInfoRoute(out string actionName, out string controllerName,
			out RouteValueDictionary routeValues)
		{
			actionName = "PaymentInfo";
			controllerName = "PayMarkStandard";
			routeValues = new RouteValueDictionary() {{"area", "SmartStore.PayMark"}};
		}
	}

	public class TransactionResult
	{
		/// <summary>
		/// Paymark Click defined unique transaction ID for this transaction.	String	8
		/// </summary>
		public string transactionId { get; set; }

		/// <summary>
		/// Used in refund, capture and cancellation transactions. Contains the transaction ID for the related (authorisation or payment) transaction.	String	8
		/// </summary>
		public string originalTransactionId { get; set; }

		/// <summary>
		/// Transaction type (PURCHASE, AUTHORISATION, STATUS_CHECK, REFUND, CAPTURE, CANCELLATION, OE_PAYMENT).	String	50
		/// </summary>
		public string type { get; set; }

		/// <summary>
		/// The Paymark Click Account ID used for processing the transaction.	Integer	8
		/// </summary>
		public int accountId { get; set; }

		/// <summary>
		/// Status of the transaction. 0 = UNKNOWN, 1 = SUCCESSFUL, 2 = DECLINED, 3 = BLOCKED, 4 = FAILED, 5 = INPROGRESS, 6 = CANCELLED.	String	50
		/// </summary>
		public string status { get; set; }

		/// <summary>
		/// Date and time when the transaction was processed.	Datetime
		/// </summary>
		public string transactionDate { get; set; }

		/// <summary>
		/// Content of this data can vary based on type of transaction. Currently when this contains a value, it is a string representing the “estimated settlement date” of the transaction.	String	100
		/// </summary>
		public string batchNumber { get; set; }

		/// <summary>
		/// Paymark Click defined unique receipt ID.	Integer	8
		/// </summary>
		public int receiptNumber { get; set; }

		/// <summary>
		/// Authorisation code returned by the Bank for this transaction.	String	100
		/// </summary>
		public string authCode { get; set; }

		/// <summary>
		/// Amount of transaction in NZD, in the format 1.23.	Decimal	20
		/// </summary>
		public decimal amount { get; set; }

		/// <summary>
		/// If the Merchant has added a surcharge % to this transaction, this is the surcharge amount for this transaction. Note: Contact Paymark to configure a surcharge for your Merchant account.	Decimal	20
		/// </summary>
		public string surcharge { get; set; }

		/// <summary>
		/// Reference used for the transaction, as defined by the Merchant.	String	50 *** We use the order Id for this in our plug in
		/// </summary>
		public string reference { get; set; }

		/// <summary>
		/// Particulars used for the transaction, as defined by the Merchant.	String	50
		/// </summary>
		public string particular { get; set; }

		/// <summary>
		/// The card type used for this transaction (MASTERCARD, VISA, AMERICAN_EXPRESS, DINERS_CLUB, QCARD).	String	50
		/// </summary>
		public string cardType { get; set; }

		/// <summary>
		/// Masked card number showing first 6 and last 4 digits of the card.	String	100
		/// </summary>
		public string cardNumber { get; set; }

		/// <summary>
		/// Expiry date of the card, in the format MMYY.	String	100
		/// </summary>
		public string cardExpiry { get; set; }

		/// <summary>
		/// The Cardholder name entered into the Paymark Click hosted web payment page.	String	100
		/// </summary>
		public string cardHolder { get; set; }

		/// <summary>
		/// Whether or not the card was stored, false = not stored, true = stored. Will always be false for Online EFTPOS payments.	Boolean	10
		/// </summary>
		public string cardStored { get; set; }

		/// <summary>
		/// Payment token ID if a payment (or card) token was used for this transaction and the payment method associated with this token is a card. Note: The Merchant can use the Merchant Portal to see payment token details when the payment method (associated with the token) is Online EFTPOS.	String	100
		/// </summary>
		public string cardToken { get; set; }

		/// <summary>
		/// The error code indicating the type of error that occurred. See Response Codes and Messages for a full listing of error codes.	String	4
		/// </summary>
		public string errorCode { get; set; }

		/// <summary>
		/// The error message explaining what the error means. See Response Codes and Messages for a full listing of error codes.	String	510
		/// </summary>
		public string errorMessage { get; set; }

		/// <summary>
		/// Response code from the acquirer to indicate the status and errors of a particular transaction processed.	String	510
		/// </summary>
		public string acquirerResponseCode { get; set; }

		/// <summary>
		/// Merchant defined reference associated with the payment (or card) token used in this transaction, if the payment method associated with this token is a card. Note: The Merchant can use the Merchant Portal to see payment token details when the payment method (associated with the token) is Online EFTPOS.	String	50
		/// </summary>
		public string tokenReference { get; set; }

		/// <summary>
		/// The marketing token registered with Paymark for the card used for this transaction. Only available if the merchantToken variable was set to 1.	String	100
		/// </summary>
		public string merchantToken { get; set; }

		/// <summary>
		/// Consumer’s personal identifier for Online EFTPOS payments.	String	100
		/// </summary>
		public string payerId { get; set; }

		/// <summary>
		/// Type of payerId that was used for Online EFTPOS payments.	String	100
		/// </summary>
		public string payerIdType { get; set; }

		/// <summary>
		/// Consumer bank to which the Online EFTPOS payment request was sent.	String	100
		/// </summary>
		public string bank { get; set; }

	}

	public enum TransactionStatus
	{
		UNKNOWN = 0,
		SUCCESSFUL = 1,
		DECLINED = 2,
		BLOCKED = 3,
		FAILED = 4,
		INPROGRESS = 5,
		CANCELLED = 6
	}
}