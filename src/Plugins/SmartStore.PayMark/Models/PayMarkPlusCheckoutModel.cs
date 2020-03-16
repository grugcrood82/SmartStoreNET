using System.Collections.Generic;
using SmartStore.Web.Framework.Modelling;

namespace SmartStore.PayMark.Models
{
	public class PayMarkPlusCheckoutModel : ModelBase
	{
		public bool UseSandbox { get; set; }
		public string BillingAddressCountryCode { get; set; }
		public string LanguageCulture { get; set; }
		public string ApprovalUrl { get; set; }
		public string ErrorMessage { get; set; }
		public string PayMarkPlusPseudoMessageFlag { get; set; }
		public string FullDescription { get; set; }
		public string PayMarkFee { get; set; }
		public string ThirdPartyFees { get; set; }
		public bool HasAnyFees { get; set; }

		public List<ThirdPartyPaymentMethod> ThirdPartyPaymentMethods { get; set; }

		public class ThirdPartyPaymentMethod
		{
			public string RedirectUrl { get; set; }
			public string MethodName { get; set; }
			public string ImageUrl { get; set; }
			public string Description { get; set; }
			public string PaymentFee { get; set; }
		}
	}
}