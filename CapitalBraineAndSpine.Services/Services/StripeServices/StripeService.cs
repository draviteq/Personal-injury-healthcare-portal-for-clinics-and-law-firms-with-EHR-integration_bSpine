using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Data.Entities.NewFolder;
using CapitalBraineAndSpine.Data.Entities.PaymentEntities;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Infrastructures.Enums;
using CapitalBraineAndSpine.Services.Services.Abstracts.StripeAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using CapitalBraineAndSpine.Services.ViewModels.StripeViewModels;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CapitalBraineAndSpine.Services.Services.StripeServices
{
    public class StripeService : IStripeService
    {
        private readonly TokenService _tokenService;
        private readonly CustomerService _customerService;
        private readonly ChargeService _chargeService;
        private readonly BaseDbContext _baseDbContext;

        public StripeService(TokenService tokenService, CustomerService customerService, ChargeService chargeService, BaseDbContext baseDbContext)
        {
            _tokenService = tokenService;
            _customerService = customerService;
            _chargeService = chargeService;
            _baseDbContext = baseDbContext;
        }

        /// <summary>
        /// Create Customer on Stripe
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CustomerResource> CreateCustomer(CreateCustomerResource resource, CancellationToken cancellationToken)
        {
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = resource.Card.Name,
                    Number = resource.Card.Number,
                    ExpYear = resource.Card.ExpiryYear,
                    ExpMonth = resource.Card.ExpiryMonth,
                    Cvc = resource.Card.Cvc
                }
            };
            var token = await _tokenService.CreateAsync(tokenOptions, null, cancellationToken);

            var customerOptions = new CustomerCreateOptions
            {
                Email = resource.Email,
                Name = resource.Name,
                Source = token.Id
            };
            var customer = await _customerService.CreateAsync(customerOptions, null, cancellationToken);

            return new CustomerResource(customer.Id, customer.Email, customer.Name);
        }

        /// <summary>
        /// Create Charge on Stripe
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ChargeResource> CreateCharge(CreateChargeResource resource, CancellationToken cancellationToken)
        {
            var chargeOptions = new ChargeCreateOptions
            {
                Currency = resource.Currency,
                Amount = resource.Amount,
                ReceiptEmail = resource.ReceiptEmail,
                Customer = resource.CustomerId,
                Description = resource.Description
            };

            var charge = await _chargeService.CreateAsync(chargeOptions, null, cancellationToken);

            return new ChargeResource(charge.Id, charge.Currency, charge.Amount, charge.CustomerId, charge.ReceiptEmail,
                charge.Description);
        }


        public async Task<string> CreateCustomerAsync(CreateCustomerWithoutCard resource)
        {

            // Search for existing customers with the same email
            var options = new CustomerListOptions
            {
                Email = resource.Email,
                Limit = 1
            };

            // Check if Customer is already created on Stripe
            var customers = await _customerService.ListAsync(options);

            // If a customer with the email exists, return the first one
            if (customers.Data.Any())
            {
                return customers.Data.First().Id.ToString();
            }
            else
            {

                // Create customer on Stripe
                var customerOptions = new CustomerCreateOptions
                {
                    Email = resource.Email,
                    Name = resource.Name,
                };


                // Create new customer on Stripe
                var customer = await _customerService.CreateAsync(customerOptions);

                // Save stripe customer Id into Database
                StripeCustomer stripeCustomer = new()
                {
                    UserId = resource.UserId,
                    StripeCustomerId = customer.Id,
                    CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                    LastUpdated = CBSHelpers.CurrentLocalDateTime()
                };

                // Save stripe Customer Id in Database
                await _baseDbContext.StripeCustomers.AddAsync(stripeCustomer);
                await _baseDbContext.SaveChangesAsync();

                return customer.Id;

            }
        }

        /// <summary>
        /// Create Payment Record in Database
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public async Task<bool> CreatePaymentAsync(Payment payment)
        {
            await _baseDbContext.Payments.AddAsync(payment);
            await _baseDbContext.SaveChangesAsync();
            return true;

        }

        /// <summary>
        /// Update Payment Status
        /// </summary>
        /// <param name="stripeSessionId"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePaymentStatusAsync(string customSessionId, string statusText)
        {

            var checkRecordExists = await _baseDbContext.Payments.Where(x => x.CustomSessionId == customSessionId).FirstOrDefaultAsync();

            if (checkRecordExists != null)
            {
                if (Enum.TryParse(statusText, out PaymentStatus status))
                {
                    int result = (int)status; // Cast the enum to its integer value
                    checkRecordExists.Status = result;
                    checkRecordExists.LastUpdated = CBSHelpers.CurrentLocalDateTime();
                }
                _baseDbContext.Update(checkRecordExists);
                await _baseDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }



    }
}
