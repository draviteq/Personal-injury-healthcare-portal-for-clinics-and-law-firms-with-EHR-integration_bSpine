using CapitalBraineAndSpine.Data.Entities.PaymentEntities;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using CapitalBraineAndSpine.Services.ViewModels.StripeViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.StripeAbstracts
{
    public interface IStripeService
    {
        Task<CustomerResource> CreateCustomer(CreateCustomerResource resource, CancellationToken cancellationToken);

        Task<string> CreateCustomerAsync(CreateCustomerWithoutCard resource);

        Task<ChargeResource> CreateCharge(CreateChargeResource resource, CancellationToken cancellationToken);

        Task<bool> CreatePaymentAsync(Payment payment);

        Task<bool> UpdatePaymentStatusAsync(string stripeSessionId, string statusText);

    }
}
