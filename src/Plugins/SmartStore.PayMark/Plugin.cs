using System;
using System.Collections.Generic;
using System.Web.Routing;
using SmartStore.Core.Logging;
using SmartStore.Core.Plugins;
using SmartStore.PayMark.Services;
using SmartStore.PayMark.Settings;
using SmartStore.Services;
using SmartStore.Services.Cms;
using SmartStore.Services.Directory;
using SmartStore.Web.Models.Catalog;
using SmartStore.Web.Models.Order;
using SmartStore.Web.Models.ShoppingCart;

namespace SmartStore.PayMark
{
    [SystemName("Widgets.PayMark")]
    [FriendlyName("PayMark")]
    public class Plugin : BasePlugin, IWidget
    {
        private readonly ICommonServices _services;
        private readonly Lazy<ICurrencyService> _currencyService;

        public Plugin(
            ICommonServices services,
			Lazy<IPayMarkService> PayMarkService,
            Lazy<ICurrencyService> currencyService)
		{
            _services = services;
            _currencyService = currencyService;

			Logger = NullLogger.Instance;
		}

		public ILogger Logger { get; set; }

		public static string SystemName => "SmartStore.PayMark";

		public override void Install()
		{
            _services.Settings.SaveSetting(new PayMarkStandardPaymentSettings());
            _services.Localization.ImportPluginResourcesFromXml(this.PluginDescriptor);

			base.Install();
		}

		public override void Uninstall()
		{
            _services.Settings.DeleteSetting<PayMarkExpressPaymentSettings>();
            _services.Settings.DeleteSetting<PayMarkDirectPaymentSettings>();
            _services.Settings.DeleteSetting<PayMarkStandardPaymentSettings>();
            _services.Settings.DeleteSetting<PayMarkPlusPaymentSettings>();
            _services.Settings.DeleteSetting<PayMarkInstalmentsSettings>();

            _services.Localization.DeleteLocaleStringResources(PluginDescriptor.ResourceRootKey);

			base.Uninstall();
		}

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                "productdetails_add_info",
                "order_summary_totals_after",
                "orderdetails_page_aftertotal",
                "invoice_aftertotal"
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, object model, int storeId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = controllerName = null;
            routeValues = new RouteValueDictionary
            {
                { "Namespaces", "SmartStore.PayMark.Controllers" },
                { "area", SystemName }
            };

            switch (widgetZone)
            {
                case "productdetails_add_info":
                {
                    var viewModel = model as ProductDetailsModel;
                    if (viewModel == null) return;
                    var price = viewModel.ProductPrice.PriceWithDiscountValue > decimal.Zero
                        ? viewModel.ProductPrice.PriceWithDiscountValue
                        : viewModel.ProductPrice.PriceValue;

                    if (price <= decimal.Zero) return;
                    actionName = "Promotion";
                    controllerName = "PayMarkInstalments";

                    // Convert price because it is in working currency.
                    price = _currencyService.Value.ConvertToPrimaryStoreCurrency(price, _services.WorkContext.WorkingCurrency);

                    routeValues.Add("origin", "productpage");
                    routeValues.Add("amount", price);
                    break;
                }
                case "order_summary_totals_after" when !(model is ShoppingCartModel viewModel) || !viewModel.IsEditable:
                    return;
                case "order_summary_totals_after":
                    actionName = "Promotion";
                    controllerName = "PayMarkInstalments";

                    routeValues.Add("origin", "cart");
                    routeValues.Add("amount", decimal.Zero);
                    break;
                case "orderdetails_page_aftertotal":
                case "invoice_aftertotal":
                {
                    if (!(model is OrderDetailsModel viewModel)) return;
                    actionName = "OrderDetails";
                    controllerName = "PayMarkInstalments";

                    routeValues.Add("orderId", viewModel.Id);
                    routeValues.Add("print", widgetZone.IsCaseInsensitiveEqual("invoice_aftertotal"));
                    break;
                }
            }
        }
    }
}
