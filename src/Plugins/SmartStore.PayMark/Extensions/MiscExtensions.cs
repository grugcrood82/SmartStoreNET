using System.Web;
using Newtonsoft.Json;
using SmartStore.Core.Domain.Customers;
using SmartStore.PayMark.Services;
using SmartStore.PayMark.Settings;
using SmartStore.Services.Common;

namespace SmartStore.PayMark
{
    internal static class MiscExtensions
	{
		public static string GetPayMarkUrl(this PayMarkSettingsBase settings)
		{
			return settings.UseSandbox ?
				"https://www.sandbox.PayMark.com/cgi-bin/webscr" :
				"https://www.PayMark.com/cgi-bin/webscr";
		}

		public static PayMarkSessionData GetPayMarkState(this HttpContextBase httpContext, string providerSystemName)
		{
            Guard.NotEmpty(providerSystemName, nameof(providerSystemName));

			var state = httpContext.GetCheckoutState();

            if (!state.CustomProperties.ContainsKey(providerSystemName))
            {
                state.CustomProperties.Add(providerSystemName, new PayMarkSessionData { ProviderSystemName = providerSystemName });
            }

			var session = state.CustomProperties.Get(providerSystemName) as PayMarkSessionData;
            return session;
		}

        public static PayMarkSessionData GetPayMarkState(
            this HttpContextBase httpContext,
            string providerSystemName,
            Customer customer,
            int storeId,
            IGenericAttributeService genericAttributeService)
        {
            Guard.NotNull(httpContext, nameof(httpContext));
            Guard.NotNull(customer, nameof(customer));
            Guard.NotNull(genericAttributeService, nameof(genericAttributeService));

            var session = httpContext.GetPayMarkState(providerSystemName);

            if (session.AccessToken.IsEmpty() || session.PaymentId.IsEmpty())
            {
                try
                {
                    var str = customer.GetAttribute<string>(providerSystemName + ".SessionData", genericAttributeService, storeId);
                    if (str.HasValue())
                    {
                        var storedSessionData = JsonConvert.DeserializeObject<PayMarkSessionData>(str);
                        if (storedSessionData != null)
                        {
                            // Only token and paymentId required.
                            session.AccessToken = storedSessionData.AccessToken;
                            session.PaymentId = storedSessionData.PaymentId;
                        }
                    }
                }
                catch { }
            }

            return session;
        }
    }
}