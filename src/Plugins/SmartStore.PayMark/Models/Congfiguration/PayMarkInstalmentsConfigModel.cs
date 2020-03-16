using System.Collections.Generic;
using System.Web.Mvc;
using SmartStore.PayMark.Services;
using SmartStore.PayMark.Settings;
using SmartStore.Web.Framework;

namespace SmartStore.PayMark.Models
{
    public class PayMarkInstalmentsConfigModel : ApiConfigurationModel
    {
        public PayMarkInstalmentsConfigModel()
        {
            TransactMode = TransactMode.AuthorizeAndCapture;
        }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.FinancingMin")]
        public decimal FinancingMin { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.FinancingMax")]
        public decimal FinancingMax { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.ProductPagePromotion")]
        public PayMarkPromotion? ProductPagePromotion { get; set; }
        public IList<SelectListItem> ProductPagePromotions { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.CartPagePromotion")]
        public PayMarkPromotion? CartPagePromotion { get; set; }
        public IList<SelectListItem> CartPagePromotions { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.PaymentListPromotion")]
        public PayMarkPromotion? PaymentListPromotion { get; set; }
        public IList<SelectListItem> PaymentListPromotions { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.Lender")]
        public string Lender { get; set; }

        //[SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.Promote")]
        //public bool Promote { get; set; }

        //[SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.PromotionWidgetZones")]
        //[UIHint("WidgetZone")]
        //public string[] PromotionWidgetZones { get; set; }

        //[SmartResourceDisplayName("Plugins.Payments.PayMarkInstalments.PromotionDisplayOrder")]
        //public int PromotionDisplayOrder { get; set; }
    }
}