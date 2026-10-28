using HealthCare.Data.Contexts;
using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Data.Entities.PaymentEntities;
using HealthCare.Service.Infrastructures.Extensions;
using HealthCare.Service.Infrastructures.Helpers;
using HealthCare.Services.Infrastructures.Enums;
using HealthCare.Services.Infrastructures.Helpers;
using HealthCare.Services.Services.Abstracts.StripeAbstracts;
using HealthCare.Services.ViewModels.StripeViewModels;
using HealthCare.Web.Infrastructures.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCare.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Authorize(Roles = "superadmin,staff,lawfirm,attorney")]
    public class StripeController : CBSBaseController
    {
        private readonly UserManager<CBSUser> _userManager;
        private readonly IStripeService _stripeService;
        private readonly IConfiguration _configuration;
        private readonly BaseDbContext _baseDbContext;

        public StripeController(UserManager<CBSUser> userManager, IStripeService stripeService
                , IConfiguration configuration
                , BaseDbContext baseDbContext

            ) : base(userManager)
        {
            _userManager = userManager;
            _stripeService = stripeService;
            _configuration = configuration;
            _baseDbContext = baseDbContext;
        }

        [HttpGet("/[Controller]")]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Success page for Stripe Payment
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]/success")]
        public IActionResult Success()
        {
            return View();
        }


        /// <summary>
        /// Cancel Page of Stripe Payment
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]/cancel")]
        public IActionResult Cancel()
        {
            return View();
        }

        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer()
        {
            // Get Logged In User details
            var user = await _userManager.GetUserAsync(HttpContext.User);

            CreateCustomerWithoutCard createCustomer = new CreateCustomerWithoutCard(
                    user.Id,
                    user.Email,
                    $"{user.FirstName} {user.LastName}"

             );

            var result = await _stripeService.CreateCustomerAsync(createCustomer);

            return Ok(GetAjaxResponse(true, string.Empty, result));
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession(int requestId, string userId)
        {
            decimal finalAmount = 0;
            var customSessionId = Guid.NewGuid().ToString();
            // Validate requestId
            var request = await _baseDbContext.PatientDataRequest.Where(x => x.Id == requestId && x.UserId == userId).FirstOrDefaultAsync();
            if (request == null)
                return Json(GetAjaxResponse(false, "Invalid request"));


            // Get user details who has requested for Patient Access
            var requestedByUserDetails = await _baseDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (requestedByUserDetails == null)
                return Json(GetAjaxResponse(false, "Invalid request by the user"));


            // Get amount from General Setting Table
            var amountSetting = await _baseDbContext.GeneralSettings.Where(x => x.SettingType == Convert.ToInt32(GeneralSettingType.DownloadDataCharges)).FirstOrDefaultAsync();

            // Check if amount availableor not
            if (amountSetting == null || !decimal.TryParse(amountSetting.SettingValue, out decimal amount) || amount <= 0)
                return Json(GetAjaxResponse(false, "Invalid amount. Please contact admin."));
            else
                finalAmount = amount;


            // Get application domain URL
            var domain = Convert.ToString(_configuration["WebsiteUrl:url"]);

            // Encrypt Request Id and Pass it to Stripe for Reference
            var requestIdEncrypt = Encryptions.Encrypt(Convert.ToString(request.Id), ConfigurationHelper.KeyForEncryption(_configuration)).ReplaceSlashWithPipe();

            // Get Logged In User details
            var loggedInUserDetails = await _userManager.GetUserAsync(HttpContext.User);

            CreateCustomerWithoutCard createCustomer = new(
                    requestedByUserDetails.Id,
                    requestedByUserDetails.Email,
                    $"{requestedByUserDetails.FirstName} {requestedByUserDetails.LastName}"

             );

            // Create customer on Stripe
            var customer = await _stripeService.CreateCustomerAsync(createCustomer);

            // Set Stripe Options
            var options = new SessionCreateOptions
            {
                Customer = customer,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount*100), // Amount in cents (e.g., $20.00)
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Profile Access",
                        },
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = $"{domain}stripe/success?session_id=" + "{CHECKOUT_SESSION_ID}",
                CancelUrl = $"{domain}stripe/cancel?session_id=" + "{CHECKOUT_SESSION_ID}",
                // Attach metadata to the PaymentIntent
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string> { { "customSessionId", customSessionId } }
                }

            };

            // Create Payment Record on Stripe
            var service = new SessionService();
            Session session = service.Create(options);

            // Create Payment record in database
            Payment payment = new()
            {
                LawFirmId = _baseDbContext.Users.Where(y => y.Id == userId).FirstOrDefault().ParentUserId,
                UserId = userId,
                StripeCustomerId = session.CustomerId,
                CustomSessionId = customSessionId,
                PatientDataRequestId = request.Id,
                Amount = finalAmount,
                Status = 0,
                Reason = string.Empty,
                IpAddress = IpHelper.GetClientIp(),
                CreatedBy = loggedInUserDetails.Id,
                CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                LastUpdated = CBSHelpers.CurrentLocalDateTime(),
            };


            // Save recor into database
            var savePayment = _stripeService.CreatePaymentAsync(payment);


            //return Ok(new { sessionId = session.Id, customerId = session.CustomerId });
            return Json(GetAjaxResponse(true, string.Empty, new { sessionId = session.Id }));
        }
    }
}
