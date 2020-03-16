using SmartStore.Core.Configuration;
using SmartStore.PayMark.Services;

namespace SmartStore.PayMark.Settings
{
    public class PayMarkInstalmentsSettings : PayMarkApiSettingsBase, ISettings
    {
        public PayMarkInstalmentsSettings()
        {
            TransactMode = TransactMode.Authorize;
            FinancingMin = 99.0M;
            FinancingMax = 5000.0M;
            ProductPagePromotion = PayMarkPromotion.FinancingExample;
            CartPagePromotion = PayMarkPromotion.FinancingExample;
        }

        public decimal FinancingMin { get; set; }
        public decimal FinancingMax { get; set; }

        //public bool Promote { get; set; }
        //public string PromotionWidgetZones { get; set; }
        //public int PromotionDisplayOrder { get; set; }

        public PayMarkPromotion? ProductPagePromotion { get; set; }
        public PayMarkPromotion? CartPagePromotion { get; set; }
        public PayMarkPromotion? PaymentListPromotion { get; set; }
        public string Lender { get; set; }

        public bool IsAmountFinanceable(decimal amount)
        {
            if (amount == decimal.Zero)
            {
                return false;
            }

            return amount >= FinancingMin && amount <= FinancingMax;
        }
    }
}