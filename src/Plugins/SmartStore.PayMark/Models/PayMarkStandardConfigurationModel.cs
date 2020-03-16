using SmartStore.ComponentModel;
using SmartStore.PayMark.Settings;
using SmartStore.Web.Framework;

namespace SmartStore.PayMark.Models
{
	public class PayMarkStandardConfigurationModel : ApiConfigurationModel
	{
		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.BusinessEmail")]
		public string BusinessEmail { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.PDTToken")]
		public string PdtToken { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.PDTValidateOrderTotal")]
		public bool PdtValidateOrderTotal { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.PdtValidateOnlyWarn")]
		public bool PdtValidateOnlyWarn { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.IsShippingAddressRequired")]
		public bool IsShippingAddressRequired { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMark.UsePayMarkAddress")]
		public bool UsePayMarkAddress { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.PassProductNamesAndTotals")]
		public bool PassProductNamesAndTotals { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.EnableIpn")]
		public bool EnableIpn { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.IpnUrl")]
		public string IpnUrl { get; set; }

		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.AccountId")]
		public string AccountId { get; set; }
		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.SecretHashKey")]
		public string SecretHashKey { get; set; }
		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.PayMarkUrl")]
		public string PayMarkUrl { get; set; }
		[SmartResourceDisplayName("Plugins.Payments.PayMarkStandard.Fields.PayMarkTransactionUrl")]
		public string PayMarkTransactionUrl { get; set; }

		public void Copy(PayMarkStandardPaymentSettings settings, bool fromSettings)
        {
            if (fromSettings)
			{
				MiniMapper.Map(settings, this);
			}
            else
			{
				MiniMapper.Map(this, settings);
				settings.BusinessEmail = BusinessEmail.TrimSafe();
			}
        }
	}
}