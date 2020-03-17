using System.Web.Mvc;
using System.Web.Routing;
using SmartStore.Web.Framework.Routing;

namespace SmartStore.PayMark
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {

            routes.MapRoute("SmartStore.PayMarkStandard",
                "Plugins/SmartStore.PayMark/{controller}/{action}",
                new { controller = "PayMarkStandard", action = "Index" },
                new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";

            //Legacay Routes
			routes.MapRoute("SmartStore.PayMarkExpress.IPN",
                 "Plugins/PaymentPayMarkExpress/IPNHandler",
                 new { controller = "PayMarkExpress", action = "IPNHandler" },
                 new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";

            routes.MapRoute("SmartStore.PayMarkDirect.IPN",
                 "Plugins/PaymentPayMarkDirect/IPNHandler",
                 new { controller = "PayMarkDirect", action = "IPNHandler" },
                 new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";

            routes.MapRoute("SmartStore.PayMarkStandard.IPN",
                 "Plugins/PaymentPayMarkStandard/IPNHandler",
                 new { controller = "PayMarkStandard", action = "IPNHandler" },
                 new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";

            routes.MapRoute("SmartStore.PayMarkStandard.PDT",
                 "Plugins/PaymentPayMarkStandard/PDTHandler",
                 new { controller = "PayMarkStandard", action = "PDTHandler" },
                 new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";

            routes.MapRoute("SmartStore.PayMarkExpress.RedirectFromPaymentInfo",
                 "Plugins/PaymentPayMarkExpress/RedirectFromPaymentInfo",
                 new { controller = "PayMarkExpress", action = "RedirectFromPaymentInfo" },
                 new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";

            routes.MapRoute("SmartStore.PayMarkStandard.CancelOrder",
                 "Plugins/PaymentPayMarkStandard/CancelOrder",
                 new { controller = "PayMarkStandard", action = "CancelOrder" },
                 new[] { "SmartStore.PayMark.Controllers" }
            )
            .DataTokens["area"] = "SmartStore.PayMark";
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
