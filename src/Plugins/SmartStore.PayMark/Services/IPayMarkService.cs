using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using SmartStore.Core.Domain.Orders;
using SmartStore.Core.Domain.Payments;
using SmartStore.Core.Domain.Stores;
using SmartStore.PayMark.Settings;
using SmartStore.Services.Payments;

namespace SmartStore.PayMark.Services
{
    public interface IPayMarkService
	{
		void AddOrderNote(PayMarkSettingsBase settings, Order order, string anyString, bool isIpn = false);

		PayMarkPaymentInstruction ParsePaymentInstruction(dynamic json);

		string CreatePaymentInstruction(PayMarkPaymentInstruction instruct);

		PaymentStatus GetPaymentStatus(string state, string reasonCode, PaymentStatus defaultStatus);

		PayMarkResponse CallApi(
            string method,
            string path,
            PayMarkApiSettingsBase settings,
            PayMarkSessionData session,
            string data);

		PayMarkResponse EnsureAccessToken(PayMarkSessionData session, PayMarkApiSettingsBase settings);

		PayMarkResponse GetPayment(PayMarkApiSettingsBase settings, PayMarkSessionData session);

        Dictionary<string, object> CreatePaymentData(
            PayMarkApiSettingsBase settings,
            PayMarkSessionData session,
            List<OrganizedShoppingCartItem> cart,
            string returnUrl,
            string cancelUrl);

        PayMarkResponse CreatePayment(
            PayMarkApiSettingsBase settings,
            PayMarkSessionData session,
            Dictionary<string, object> data);

		PayMarkResponse PatchShipping(
			PayMarkApiSettingsBase settings,
			PayMarkSessionData session,
			List<OrganizedShoppingCartItem> cart);

		PayMarkResponse ExecutePayment(PayMarkApiSettingsBase settings, PayMarkSessionData session);

		PayMarkResponse Refund(PayMarkApiSettingsBase settings, PayMarkSessionData session, RefundPaymentRequest request);

		PayMarkResponse Capture(PayMarkApiSettingsBase settings, PayMarkSessionData session, CapturePaymentRequest request);

		PayMarkResponse Void(PayMarkApiSettingsBase settings, PayMarkSessionData session, VoidPaymentRequest request);

		PayMarkResponse UpsertCheckoutExperience(PayMarkApiSettingsBase settings, PayMarkSessionData session, Store store);

		PayMarkResponse DeleteCheckoutExperience(PayMarkApiSettingsBase settings, PayMarkSessionData session);

		PayMarkResponse CreateWebhook(PayMarkApiSettingsBase settings, PayMarkSessionData session, string url);

		PayMarkResponse DeleteWebhook(PayMarkApiSettingsBase settings, PayMarkSessionData session);

		HttpStatusCode ProcessWebhook(
			PayMarkApiSettingsBase settings,
			NameValueCollection headers,
			string rawJson,
			string providerSystemName);

        #region Credit

        FinancingOptions GetFinancingOptions(
            PayMarkInstalmentsSettings settings,
            PayMarkSessionData session,
            string origin,
            decimal amount,
            PayMarkPromotion? promotion = null);

        #endregion
    }
}