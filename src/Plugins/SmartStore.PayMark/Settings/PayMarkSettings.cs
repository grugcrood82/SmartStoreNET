using System.Collections.Generic;
using SmartStore.Core.Configuration;

namespace SmartStore.PayMark.Settings
{
	public abstract class PayMarkSettingsBase
    {
		public PayMarkSettingsBase()
		{
			IpnChangesPaymentStatus = true;
			AddOrderNotes = true;
		}

		public bool UseSandbox { get; set; }

		public bool AddOrderNotes { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
		/// </summary>
		public bool AdditionalFeePercentage { get; set; }
        
        public decimal AdditionalFee { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether an IPN should change the payment status
		/// </summary>
		public bool IpnChangesPaymentStatus { get; set; }
	}

    public class PayMarkApiSettingsBase : PayMarkSettingsBase
	{
		public TransactMode TransactMode { get; set; }
		public string ApiAccountName { get; set; }
		public string ApiAccountPassword { get; set; }
		public string Signature { get; set; }

		/// <summary>
		/// PayMark client id
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// PayMark secret
		/// </summary>
		public string Secret { get; set; }

		/// <summary>
		/// PayMark experience profile id
		/// </summary>
		public string ExperienceProfileId { get; set; }

		/// <summary>
		/// PayMark webhook id
		/// </summary>
		public string WebhookId { get; set; }
	}


    public class PayMarkDirectPaymentSettings : PayMarkApiSettingsBase, ISettings
    {
		public PayMarkDirectPaymentSettings()
		{
			TransactMode = TransactMode.Authorize;
		}
    }

    public class PayMarkExpressPaymentSettings : PayMarkApiSettingsBase, ISettings 
    {
		public PayMarkExpressPaymentSettings()
		{
            TransactMode = TransactMode.Authorize;
		}

        /// <summary>
        /// Determines whether the checkout button is displayed beneath the cart
        /// </summary>
        //public bool DisplayCheckoutButton { get; set; }

		/// <summary>
		/// Specifies whether to display the checkout button in mini shopping cart
		/// </summary>
		public bool ShowButtonInMiniShoppingCart { get; set; }

		/// <summary>
		/// Determines whether the shipment address has  to be confirmed by PayMark 
		/// </summary>
		public bool ConfirmedShipment { get; set; }

        /// <summary>
        /// Determines whether the shipment address is transmitted to PayMark
        /// </summary>
        public bool NoShipmentAddress { get; set; }

        /// <summary>
        /// Callback timeout
        /// </summary>
        public int CallbackTimeout { get; set; }

        /// <summary>
        /// Default shipping price
        /// </summary>
        public decimal DefaultShippingPrice { get; set; }
    }

	public class PayMarkPlusPaymentSettings : PayMarkApiSettingsBase, ISettings
	{
        public PayMarkPlusPaymentSettings()
        {
            TransactMode = TransactMode.AuthorizeAndCapture;
        }

        /// <summary>
        /// Specifies other payment methods to be offered in payment wall
        /// </summary>
        public List<string> ThirdPartyPaymentMethods { get; set; }

		/// <summary>
		/// Specifies whether to display the logo of a third party payment method
		/// </summary>
		public bool DisplayPaymentMethodLogo { get; set; }

		/// <summary>
		/// Specifies whether to display the description of a third party payment method
		/// </summary>
		public bool DisplayPaymentMethodDescription { get; set; }
	}

	public class PayMarkStandardPaymentSettings : PayMarkSettingsBase, ISettings
    {
		public PayMarkStandardPaymentSettings()
		{
            EnableIpn = true;
			IsShippingAddressRequired = true;
		}

		public string ApiAccountPassword { get; set; }
		public string ClientId { get; set; }
		public string AccountId { get; set; }
		public string SecretHashKey { get; set; }
		public string PaymentCommand { get; set; } = "_xclick";

		public string PayMarkUrl { get; set; } =  "https://uat.paymarkclick.co.nz/api/webpayments/paymentservice/rest/WPRequest";
		public string PayMarkTransactionUrl { get; set; } =  "https://uat.paymarkclick.co.nz/api/transaction/search/";

		public string PrimaryStoreCurrencyCode { get; set; }

		
        public string BusinessEmail { get; set; }
        public string PdtToken { get; set; }
        public bool PassProductNamesAndTotals { get; set; }
        public bool PdtValidateOrderTotal { get; set; }
		public bool PdtValidateOnlyWarn { get; set; }
        public bool EnableIpn { get; set; }
        public string IpnUrl { get; set; }

		/// <summary>
		/// Specifies whether to use PayMark shipping address. <c>true</c> use PayMark address, <c>false</c> use checkout address.
		/// </summary>
		public bool UsePayMarkAddress { get; set; }

		/// <summary>
		/// Specifies whether a shipping address is required.
		/// </summary>
		public bool IsShippingAddressRequired { get; set; }
	}


	/// <summary>
	/// Represents payment processor transaction mode
	/// </summary>
	public enum TransactMode
    {
        Authorize = 1,
        AuthorizeAndCapture = 2
    }
}
