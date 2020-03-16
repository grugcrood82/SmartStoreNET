using SmartStore.Web.Framework.Modelling;

namespace SmartStore.PayMark.Models
{
    public class PayMarkExpressPaymentInfoModel : ModelBase
    {
        public PayMarkExpressPaymentInfoModel()
        {
            
        }

        public bool CurrentPageIsBasket { get; set; }

        public string SubmitButtonImageUrl { get; set; }

    }
}