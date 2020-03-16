using System;
using System.Collections.Generic;
using FluentValidation;
using SmartStore.Core.Localization;
using SmartStore.PayMark.Settings;
using SmartStore.Web.Framework;
using SmartStore.Web.Framework.Validators;

namespace SmartStore.PayMark.Models
{
    public class PayMarkPlusConfigurationModel : ApiConfigurationModel
    {
        public PayMarkPlusConfigurationModel()
        {
            TransactMode = TransactMode.AuthorizeAndCapture;
        }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkPlus.ThirdPartyPaymentMethods")]
        public List<string> ThirdPartyPaymentMethods { get; set; }
        public IList<ExtendedSelectListItem> AvailableThirdPartyPaymentMethods { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkPlus.DisplayPaymentMethodLogo")]
        public bool DisplayPaymentMethodLogo { get; set; }

        [SmartResourceDisplayName("Plugins.Payments.PayMarkPlus.DisplayPaymentMethodDescription")]
        public bool DisplayPaymentMethodDescription { get; set; }
    }


    public class PayMarkApiConfigValidator : SmartValidatorBase<ApiConfigurationModel>
    {
        public PayMarkApiConfigValidator(Localizer T, Func<string, bool> addRule)
        {
            if (addRule("ClientId"))
            {
                RuleFor(x => x.ClientId).NotEmpty();
            }

            if (addRule("Secret"))
            {
                RuleFor(x => x.Secret).NotEmpty();
            }
        }
    }
}