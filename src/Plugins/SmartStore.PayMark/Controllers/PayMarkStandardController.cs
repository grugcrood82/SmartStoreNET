using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using SmartStore.Core.Domain.Payments;
using SmartStore.Core.Logging;
using SmartStore.PayMark.Models;
using SmartStore.PayMark.Settings;
using SmartStore.Services.Orders;
using SmartStore.Services.Payments;
using SmartStore.Web.Framework.Controllers;
using SmartStore.Web.Framework.Security;
using SmartStore.Web.Framework.Settings;

namespace SmartStore.PayMark.Controllers
{
	public class PayMarkStandardController : PayMarkControllerBase<PayMarkStandardPaymentSettings>
	{
        public PayMarkStandardController(
			IPaymentService paymentService,
			IOrderService orderService,
			IOrderProcessingService orderProcessingService) : base(
				paymentService,
				orderService,
				orderProcessingService)
		{
		}

        protected override string ProviderSystemName => PayMarkStandardProvider.SystemName;

        [AdminAuthorize, ChildActionOnly, LoadSetting]
		public ActionResult Configure(PayMarkStandardPaymentSettings settings, int storeScope)
		{
            var model = new PayMarkStandardConfigurationModel();
            model.Copy(settings, true);

			PrepareConfigurationModel(model, storeScope);

            return View(model);
		}

		[HttpPost, AdminAuthorize, ChildActionOnly]
		public ActionResult Configure(PayMarkStandardConfigurationModel model, FormCollection form)
		{
			var storeDependingSettingHelper = new StoreDependingSettingHelper(ViewData);
			var storeScope = this.GetActiveStoreScopeConfiguration(Services.StoreService, Services.WorkContext);
			var settings = Services.Settings.LoadSetting<PayMarkStandardPaymentSettings>(storeScope);

			if (!ModelState.IsValid)
			{
				return Configure(settings, storeScope);
			}

			ModelState.Clear();
			model.Copy(settings, false);

			using (Services.Settings.BeginScope())
			{
				storeDependingSettingHelper.UpdateSettings(settings, form, storeScope, Services.Settings);
			}

			using (Services.Settings.BeginScope())
			{
				// Multistore context not possible, see IPN handling.
				Services.Settings.SaveSetting(settings, x => x.UseSandbox, 0, false);
			}

			NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));

			return RedirectToConfiguration(PayMarkStandardProvider.SystemName, false);
		}

		public ActionResult PaymentInfo()
		{
			return PartialView();
		}

		[NonAction]
		public override IList<string> ValidatePaymentForm(FormCollection form)
		{
			var warnings = new List<string>();
			return warnings;
		}

		[NonAction]
		public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
		{
			var paymentInfo = new ProcessPaymentRequest();
			return paymentInfo;
		}

		[ValidateInput(false)]
		public ActionResult PDTHandler(FormCollection form)
		{
			var transactionid = form.GetValue("TransactionId").AttemptedValue;
			
			TransactionResult values;
			var orderNumberGuid = Guid.Empty;
			var orderNumber = string.Empty;
			var total = decimal.Zero;
			string response;

			var provider = PaymentService.LoadPaymentMethodBySystemName(PayMarkStandardProvider.SystemName, true);
			if (!(provider?.Value is PayMarkStandardProvider processor))
			{
				Logger.Warn(null, T("Plugins.Payments.PayMark.NoModuleLoading", "PDTHandler"));
				return RedirectToAction("Completed", "Checkout", new { area = "" });
			}

			var settings = Services.Settings.LoadSetting<PayMarkStandardPaymentSettings>();

			if (processor.GetPDTDetails(transactionid, settings, out values, out response))
			{
				orderNumber = values.reference;

				try
				{
					orderNumberGuid = new Guid(orderNumber);
				}
				catch { }

				var order = OrderService.GetOrderByGuid(orderNumberGuid);

				if (order != null)
				{
					try
					{
						total = values.amount;
					}
					catch (Exception ex)
					{
						Logger.Error(ex, T("Plugins.Payments.PayMarkStandard.FailedGetGross"));
					}


					var paymentNote = T("Plugins.Payments.PayMarkStandard.PaymentNote",
						total, "NZD", values.status, values.status, values.errorMessage, values.transactionId,
						values.type, values.payerId, values.accountId, values.receiptNumber, values.particular);

					OrderService.AddOrderNote(order, paymentNote);

					// mark order as paid
					var newPaymentStatus = GetPaymentStatus(values.status, values.type, total, order.OrderTotal);

					if (newPaymentStatus == PaymentStatus.Paid)
					{
						// note, order can be marked as paid through IPN
						if (order.AuthorizationTransactionId.IsEmpty())
						{
							order.AuthorizationTransactionId = order.CaptureTransactionId = values.transactionId;
							order.AuthorizationTransactionResult = order.CaptureTransactionResult = "Success";

							OrderService.UpdateOrder(order);
						}

						if (OrderProcessingService.CanMarkOrderAsPaid(order))
						{
							OrderProcessingService.MarkOrderAsPaid(order);
						}
					}
				}

				return RedirectToAction("Completed", "Checkout", new { area = "" });
			}

			try
			{
				orderNumber = values.reference;
				orderNumberGuid = new Guid(orderNumber);

				var order = OrderService.GetOrderByGuid(orderNumberGuid);
				OrderService.AddOrderNote(order, "{0} {1}".FormatInvariant(T("Plugins.Payments.PayMarkStandard.PdtFailed"), response));
			}
			catch
			{
				// ignored
			}

			var fpm = JsonConvert.DeserializeObject<FailedPaymentModel>(response);
			return RedirectToAction("FailedPayment","PayMarkStandard",fpm);
		}

		public ActionResult CancelOrder(FormCollection form)
		{
			var order = OrderService.SearchOrders(Services.StoreContext.CurrentStore.Id, Services.WorkContext.CurrentCustomer.Id, null, null, null, null, null, null, null, null, 0, 1)
				.FirstOrDefault();

			if (order != null)
			{
				return RedirectToAction("Details", "Order", new { id = order.Id, area = "" });
			}

			return RedirectToAction("Index", "Home", new { area = "" });
		}

		public ActionResult FailedPayment(FailedPaymentModel fpm)
		{
			return View("PaymentFailed", fpm);
		}
	}
	
	public class FailedPaymentModel
	{
		public string transactionId { get; set; }
		public object originalTransactionId { get; set; }
		public string type { get; set; }
		public int accountId { get; set; }
		public string status { get; set; }
		public DateTime transactionDate { get; set; }
		public string batchNumber { get; set; }
		public int receiptNumber { get; set; }
		public string authCode { get; set; }
		public double amount { get; set; }
		public double surcharge { get; set; }
		public string reference { get; set; }
		public string particular { get; set; }
		public string cardType { get; set; }
		public string cardNumber { get; set; }
		public string cardExpiry { get; set; }
		public string cardHolder { get; set; }
		public bool cardStored { get; set; }
		public object cardToken { get; set; }
		public string errorCode { get; set; }
		public string errorMessage { get; set; }
		public string acquirerResponseCode { get; set; }
		public object tokenReference { get; set; }
		public string merchantToken { get; set; }
		public object payerId { get; set; }
		public object payerIdType { get; set; }
		public object bank { get; set; }
	}
}