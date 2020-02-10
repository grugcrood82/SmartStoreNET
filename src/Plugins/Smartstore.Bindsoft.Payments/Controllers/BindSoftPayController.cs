using System.Collections.Generic;
using System.Web.Mvc;
using SmartStore.PayPal.Models;
using SmartStore.Services.Payments;
using SmartStore.Web.Framework.Controllers;

namespace Smartstore.Bindsoft.Payments.Controllers
{
    public  class BindSoftPayController:PaymentControllerBase
    {
        protected  string ProviderSystemName { get; }
        protected void PrepareConfigurationModel(ApiConfigurationModel model, int storeScope)
        {
            var store = storeScope == 0
                ? Services.StoreContext.CurrentStore
                : Services.StoreService.GetStoreById(storeScope);

            model.PrimaryStoreCurrencyCode = store.PrimaryStoreCurrency.CurrencyCode;
        }


        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            throw new System.NotImplementedException();
        }

        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            throw new System.NotImplementedException();
        }
    }
}