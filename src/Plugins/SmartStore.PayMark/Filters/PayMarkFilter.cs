using System;
using System.Web.Mvc;
using SmartStore.PayMark.Settings;
using SmartStore.Web.Framework.UI;

namespace SmartStore.PayMark.Filters
{
    //public class PayMarkFilter : IActionFilter
    //{
    //    private readonly Lazy<IWidgetProvider> _widgetProvider;
    //    private readonly Lazy<PayMarkInstalmentsSettings> _PayMarkInstalmentsSettings;

    //    public PayMarkFilter(
    //        Lazy<IWidgetProvider> widgetProvider,
    //        Lazy<PayMarkInstalmentsSettings> PayMarkInstalmentsSettings)
    //    {
    //        _widgetProvider = widgetProvider;
    //        _PayMarkInstalmentsSettings = PayMarkInstalmentsSettings;
    //    }

    //    public void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //    }

    //    public void OnActionExecuted(ActionExecutedContext filterContext)
    //    {
    //        if (filterContext.IsChildAction)
    //            return;

    //        if (!filterContext.Result.IsHtmlViewResult())
    //            return;

    //        if (filterContext.HttpContext.Request.HttpMethod != "GET")
    //            return;

    //        // Promotion for instalments payment.
    //        if (_PayMarkInstalmentsSettings.Value.Promote && _PayMarkInstalmentsSettings.Value.PromotionWidgetZones.HasValue())
    //        {
    //            _widgetProvider.Value.RegisterAction(
    //                _PayMarkInstalmentsSettings.Value.PromotionWidgetZones.SplitSafe(","),
    //                "Promote",
    //                "PayMarkInstalments",
    //                new { area = Plugin.SystemName },
    //                _PayMarkInstalmentsSettings.Value.PromotionDisplayOrder);
    //        }
    //    }
    //}
}