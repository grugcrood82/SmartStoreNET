using System;
using System.Web.Routing;
using Smartstore.Bindsoft.Payments.Controllers;
using SmartStore.Core.Plugins;
using SmartStore.Services.Payments;

namespace Smartstore.Bindsoft.Payments.Providers
{
    [SystemName("Payments.Bindsoft.Payments.PayMark")]
    [FriendlyName("Bindsoft Payments")]
    [DisplayOrder(10)]
    public class BindsoftPaymentProvider:PaymentMethodBase
    {
        public override ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public override void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            throw new NotImplementedException();
        }

        public override void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            throw new NotImplementedException();
        }

        public override Type GetControllerType()
        {
            return typeof(BindSoftPayController);
        }

        public override PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
    }
}