using System.Web.Routing;
using SmartStore.Core.Configuration;
using SmartStore.Core.Logging;
using SmartStore.Core.Plugins;
using SmartStore.Services;

namespace Smartstore.Bindsoft.Payments
{
    [FriendlyName("Bindsoft Pay")]
    [DisplayOrder(-1)]
    public class BindsoftPlugin:BasePlugin, IConfigurable
    {
        private readonly ICommonServices _commonServices;

        public BindsoftPlugin(ICommonServices commonServices)
        {
            _commonServices = commonServices;
        }
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            throw new System.NotImplementedException();
        }
        
        public ILogger Logger { get; set; }
        
        public static string SystemName => "SmartStore.BinsoftPay";

        public override void Install()
        {
            _commonServices.Settings.SaveSetting(new BindsoftPlugInSettings());

            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
            _commonServices.Settings.DeleteSetting<BindsoftPlugInSettings>();

        }
    }

    


    public class BindsoftPlugInSettings : ISettings
    {
        public string ApiPassword { get; set; }
        public string ClientId { get; set; }
        public string AccountId { get; set; }
        public string SecretHashKey { get; set; }
        public string PaymentCommand { get; set; } = "_xclick";
    }
}