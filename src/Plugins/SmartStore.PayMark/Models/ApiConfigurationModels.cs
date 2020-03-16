using System.ComponentModel.DataAnnotations;
using SmartStore.ComponentModel;
using SmartStore.PayMark.Settings;
using SmartStore.Web.Framework;
using SmartStore.Web.Framework.Modelling;

namespace SmartStore.PayMark.Models
{
    public abstract class ApiConfigurationModel : ModelBase
	{
		public string PrimaryStoreCurrencyCode { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.UseSandbox")]
		public bool UseSandbox { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.IpnChangesPaymentStatus")]
		public bool IpnChangesPaymentStatus { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.TransactMode")]
		public TransactMode TransactMode { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.ApiAccountName")]
		public string ApiAccountName { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.ApiAccountPassword")]
		[DataType(DataType.Password)]
		public string ApiAccountPassword { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.Signature")]
		public string Signature { get; set; }

		[SmartResourceDisplayName("Plugins.SmartStore.PayMark.ClientId")]
		public string ClientId { get; set; }

		[SmartResourceDisplayName("Plugins.SmartStore.PayMark.Secret")]
		public string Secret { get; set; }

		[SmartResourceDisplayName("Plugins.SmartStore.PayMark.ExperienceProfileId")]
		public string ExperienceProfileId { get; set; }

		[SmartResourceDisplayName("Plugins.SmartStore.PayMark.WebhookId")]
		public string WebhookId { get; set; }

		[SmartResourceDisplayName("Admin.Configuration.Payment.Methods.AdditionalFee")]
		public decimal AdditionalFee { get; set; }

		[SmartResourceDisplayName("Admin.Configuration.Payment.Methods.AdditionalFeePercentage")]
		public bool AdditionalFeePercentage { get; set; }
	}

    public class PayMarkDirectConfigurationModel : ApiConfigurationModel
    {
        public void Copy(PayMarkDirectPaymentSettings settings, bool fromSettings)
        {
            if (fromSettings)
			{
				MiniMapper.Map(settings, this);
			}
			else
			{
				MiniMapper.Map(this, settings);
				settings.ApiAccountName = ApiAccountName.TrimSafe();
				settings.ApiAccountPassword = ApiAccountPassword.TrimSafe();
				settings.ClientId = ClientId.TrimSafe();
				settings.ExperienceProfileId = ExperienceProfileId.TrimSafe();
				settings.Secret = Secret.TrimSafe();
				settings.Signature = Signature.TrimSafe();
				settings.WebhookId = WebhookId.TrimSafe();
			}
        }
    }

    public class PayMarkExpressConfigurationModel : ApiConfigurationModel
    {
		[SmartResourceDisplayName("Plugins.Payments.PayMarkExpress.Fields.ShowButtonInMiniShoppingCart")]
		public bool ShowButtonInMiniShoppingCart { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkExpress.Fields.ConfirmedShipment")]
        public bool ConfirmedShipment { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkExpress.Fields.NoShipmentAddress")]
        public bool NoShipmentAddress { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkExpress.Fields.CallbackTimeout")]
        public int CallbackTimeout { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkExpress.Fields.DefaultShippingPrice")]
        public decimal DefaultShippingPrice { get; set; }

        public void Copy(PayMarkExpressPaymentSettings settings, bool fromSettings)
        {
            if (fromSettings)
			{
				MiniMapper.Map(settings, this);
			}
            else
			{
				MiniMapper.Map(this, settings);
				settings.ApiAccountName = ApiAccountName.TrimSafe();
				settings.ApiAccountPassword = ApiAccountPassword.TrimSafe();
				settings.Signature = Signature.TrimSafe();
			}
        }
    }    
}