using HealthCare.Web.Infrastructures.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Claims;
using System.Security.Cryptography;

namespace HealthCare.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<CBSUser> _userManager;
        private readonly SignInManager<CBSUser> _signInManager;
        private readonly RoleManager<CBSRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly BaseDbContext _baseDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IAuditLogService _auditLogService;

        public AccountController(UserManager<CBSUser> userManager,
            SignInManager<CBSUser> signInManager,
            RoleManager<CBSRole> roleManager,
            IEmailService emailService,
            IUserService userService,
            BaseDbContext baseDbContext,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            IAuditLogService auditLogService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _userService = userService;
            _baseDbContext = baseDbContext;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _auditLogService = auditLogService;

        }


        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        /// <summary>
        /// Login with Email and Password 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            model.ReturnUrl = returnUrl;
            model.Password = model.Password;

            if (ModelState.IsValid)
            {

                // sign-in logic here
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Check if account is deleted                
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }
                else if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account is not active.");
                    return View(model);
                }
                else
                {
                    // Retrieve roles assigned to the user
                    var roles = await _userManager.GetRolesAsync(user);

                    // Add Roles in Claims
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim("UserId",user.Id)
                    };

                    // Add roles to claims
                    foreach (var role in roles)
                    {
                        /*
                        if (role == Roles.Attorney)
                        {
                            return View("underconstruction");
                        }
                        */
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    // Create the identity with the user claims
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Create the principal with the claims identity
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    await _auditLogService.AuditLogsAsync(user.Id, "Login Successfullty");

                    // Reset AccessFailedCount
                    // await _userManager.ResetAccessFailedCountAsync(user);

                    model.IsSuccessLogin = true;
                    model.UserId = user.Id;

                    // If there is a ReturnUrl, redirect to it; otherwise, redirect to the default page
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("index", "dashboard");
                }
                else
                {
                    // Add audit log
                    await _auditLogService.AuditLogsAsync(user.Id, "Login Failed");

                    // add to validation summary 
                    ModelState.AddModelError(string.Empty, "Login failed.");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Register page is only for development
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Register(string key, string? type = null)
        {

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                StateList = await _userService.StateListAsync(),
            };

            ViewData["type"] = type;
            return View(registerViewModel);
        }



        /// <summary>
        /// Register
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string? type = null)
        {
            ViewData["type"] = type;
            model.RoleName = type;

            //if (ModelState.IsValid)
            //{
            var user = new CBSUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Zip = model.Zip,
                PhoneNumber = model.PhoneNumber,
                CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                UpdatedDate = CBSHelpers.CurrentLocalDateTime(),
                IsActive = true
                //UserCreatedBy = (await GetCurrentUserAsync()).Id,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roles = new CBSRole { Name = Roles.SuperAdmin, Caption = Roles.SuperAdmin, Prefix = RolesPrefix.SuperAdminPrefix, DisplayName = RoleDisplayName.SuperAdmin };

                //if (type == Roles.CBSAdmin.ToLower())
                if (type == Roles.CBSAdmin.ToLower())
                    roles = new CBSRole { Name = Roles.CBSAdmin, Caption = Roles.CBSAdmin, Prefix = RolesPrefix.CBSAdminPrefix, DisplayName = RoleDisplayName.CBSAdmin };

                if (type == Roles.CBSStaff.ToLower())
                    roles = new CBSRole { Name = Roles.CBSStaff, Caption = Roles.CBSStaff, Prefix = RolesPrefix.CBSStaffPrefix, DisplayName = RoleDisplayName.CBSStaff };

                if (type == Roles.LawFirm.ToLower())
                    roles = new CBSRole { Name = Roles.LawFirm, Caption = Roles.LawFirm, Prefix = RolesPrefix.LawFirmPrefix, DisplayName = RoleDisplayName.LawFirm };

                if (type == Roles.Attorney.ToLower())
                    roles = new CBSRole { Name = Roles.Attorney, Caption = Roles.Attorney, Prefix = RolesPrefix.AttorneyPrefix, DisplayName = RoleDisplayName.Attorney };

                if (!await _roleManager.RoleExistsAsync(roles.Name))
                    await _roleManager.CreateAsync(roles);

                if (!await _userManager.IsInRoleAsync(user, roles.Name))
                    await _userManager.AddToRoleAsync(user, roles.Name);

                var userDetails = _baseDbContext.Users.Where(x => x.Id == user.Id).FirstOrDefault();
                if (userDetails != null)
                {
                    // Get user role
                    var userRole = await _userManager.GetRolesAsync(user);

                    // Get Max Prefix Values of role from users
                    var usersInRole = await _userManager.GetUsersInRoleAsync(roles.Name);

                    // Get max id
                    var maxPrefixId = usersInRole.OrderByDescending(x => x.PrefixVal).Max(x => x.PrefixVal);

                    // Create customUserId
                    var customUserId = roles != null ? $"{roles.Prefix}{(maxPrefixId > 0 ? maxPrefixId + 1 : 1)}" : string.Empty;

                    userDetails.CustomUserId = customUserId;
                    userDetails.PrefixVal = maxPrefixId > 0 ? maxPrefixId + 1 : 1;
                    _baseDbContext.Users.Update(userDetails);
                    await _baseDbContext.SaveChangesAsync();

                    // Base url from AppSettings
                    var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
                    // Send template                        
                    var tbl = new Hashtable
                        {
                            { "@FirstName", model.FirstName },
                            { "@LastName", model.LastName },
                            { "@Username", model.Email },
                            { "@baseurl", $"{baseUrl}account/login" }

                        };

                    string bodyContent = _emailService.GetMailBody("NotifyUser.html", tbl);

                    EmailViewModel emailModel = new EmailViewModel { To = user.Email, Subject = "Your login details", Body = bodyContent };

                    await _emailService.SendEmailAsync(emailModel);
                }
                return View("RegisterConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            //}                       

            return View(model);
        }


        /// <summary>
        /// Login with Email via OTP
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult LoginWithEmail()
        {
            return View();
        }

        /// <summary>
        /// Login with Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithEmail(string email, string? returnUrl = null)
        {
            returnUrl = string.Empty;
            ViewData["ReturnUrl"] = returnUrl;
            var loginEmailViewModel = new LoginEmailViewModel();
            if (ModelState.IsValid)
            {
                // sign-in logic here
                var user = await _userManager.FindByEmailAsync(email);

                // Check if account is deleted                
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User does not exists.");
                    return View();
                }
                else
                {
                    // Generate OTP
                    var otp = CBSHelpers.GenerateRandomOTP(6);

                    // Get Expiration time
                    int expireTime = Convert.ToInt32(_configuration["OtpExpireTime:time"]);
                    var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
                    var mainLogo = $"{baseUrl}assets/images/brand-logos/logo.png";

                    if (user != null)
                    {
                        user.Otp = Convert.ToInt32(otp);
                        user.OtpCreated = CBSHelpers.CurrentLocalDateTime();
                        user.OtpUsed = false;

                        loginEmailViewModel.UserId = user.Id;
                    }

                    // Update OTP details in database
                    _baseDbContext.Update(user);
                    await _baseDbContext.SaveChangesAsync();

                    // Email template                        
                    var tbl = new Hashtable
                            {
                                { "@FirstName", user.FirstName },
                                { "@LastName", user.LastName },
                                { "@expireTime", expireTime },
                                { "@otp", otp },
                                { "@mainlogo",  mainLogo},
                                { "@userId",user.Id},
                                { "@baseurl",  baseUrl}

                            };

                    string bodyContent = _emailService.GetMailBody("SendOTP.html", tbl);

                    // Send OTP on Email
                    EmailViewModel emailModel = new EmailViewModel { To = user.Email, Subject = $"Your one-time-password for login", Body = bodyContent };
                    await _emailService.SendEmailAsync(emailModel);

                    // Log 
                    await _auditLogService.AuditLogsAsync(user.Id, "Login OTP send in Email");
                }
            }
            return View("ValidateOtp", loginEmailViewModel);
        }


        /// <summary>
        /// Validate OTP
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateOtp(int otp, string userId)
        {

            var redirectToUrl = "/dashboard";

            // Get loggedIn user 
            var userDetails = await _baseDbContext.Users.Where(x => x.Id == userId && x.Otp == otp).FirstOrDefaultAsync();

            if (userDetails == null)
                return Json(new { success = true, message = "Invalid otp. Please try again" });

            // Get OTP validity
            int otpValidity = Convert.ToInt32(_configuration["OtpExpireTime:time"]);

            // Otp will be expire on
            var otpCreatedTime = userDetails.OtpCreated.GetValueOrDefault().AddSeconds(otpValidity);

            // Get current local time
            var currentTime = CBSHelpers.CurrentLocalDateTime();


            // Check time difference
            var timeDifference = (otpCreatedTime - currentTime).TotalSeconds;

            if (timeDifference < 0 || timeDifference > otpValidity)
                return Json(new { success = true, message = "OTP has expired or is invalid" });
            else
            {
                // Update OTP used in DB
                userDetails.OtpUsed = true;
                _baseDbContext.Update(userDetails);
                await _baseDbContext.SaveChangesAsync();
                var userDetail = await _userManager.FindByEmailAsync(userDetails.Email);


                // Retrieve roles assigned to the user
                var roles = await _userManager.GetRolesAsync(userDetail);

                // Add Roles in Claims
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name,$"{userDetail.FirstName} {userDetail.LastName}"),
                    new Claim(ClaimTypes.Email,userDetail.Email)
                    };

                // Add roles to claims
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Create the identity with the user claims
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create the principal with the claims identity
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await _signInManager.SignInAsync(userDetail, isPersistent: true);

                // Log 
                await _auditLogService.AuditLogsAsync(userDetails.Id, "Login via OTP");
            }

            // Get User Roles
            var userRole = await _userManager.GetRolesAsync(userDetails);

            return Json(new
            {
                success = true,
                message = "",
                url = redirectToUrl
            });
        }


        /// <summary>
        /// ResendOtp is calles when previous OTP is expire
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>


        [HttpGet]
        public async Task<IActionResult> ResendOtp(string userId)
        {
            // Get user details           
            var userDetails = await _baseDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (userDetails == null)
                return Json(new { success = true, message = "Invalid user" });

            // Generate OTP and expiration time
            var otp = CBSHelpers.GenerateRandomOTP(6);
            var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
            // Get OTP expiration time
            int otpExpireTime = Convert.ToInt32(_configuration["OtpExpireTime:time"]);

            // Update new OTP in database
            userDetails.Otp = Convert.ToInt32(otp);
            userDetails.OtpUsed = true;
            userDetails.OtpCreated = CBSHelpers.CurrentLocalDateTime();
            userDetails.OtpUsed = false;


            _baseDbContext.Update(userDetails);
            await _baseDbContext.SaveChangesAsync();

            //// Get Expiration time
            int expireTime = Convert.ToInt32(_configuration["OtpExpireTime:time"]);
            baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
            var mainLogo = $"{baseUrl}assets/images/brand-logos/logo.png";

            // Email template                        
            var tbl = new Hashtable
                            {
                                { "@FirstName", userDetails.FirstName },
                                { "@LastName", userDetails.LastName },
                                { "@expireTime", expireTime },
                                { "@otp", otp },
                                { "@mainlogo",  mainLogo},
                                { "@baseurl",  baseUrl}
                            };

            string bodyContent = _emailService.GetMailBody("SendOTP.html", tbl);

            // Send OTP on Email
            EmailViewModel emailModel = new EmailViewModel { To = userDetails.Email, Subject = $"Your one-time-password for login", Body = bodyContent };
            await _emailService.SendEmailAsync(emailModel);

            // Log 
            await _auditLogService.AuditLogsAsync(userDetails.Id, "OTP re-send for Login");

            LoginViewModel model = new LoginViewModel();
            model.IsSuccessLogin = true;
            model.ResendOtp = true;

            return Json(new { success = true, message = "" });

        }
        /// <summary>
        /// Logout the user Id 
        /// </summary>
        /// <returns></returns>


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = string.Empty;
            if (User.Identity.IsAuthenticated)
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _signInManager.SignOutAsync();

            // Log 
            await _auditLogService.AuditLogsAsync(userId, "Log Out Successfully.");

            // Create Activity log            
            // await _commonService.CreateActivityLog(ActivityConstants.LoggedOut, userId, HttpContext.GetClientIpAddress());
            // Activity log End

            return Json(new { success = true });

        }

        /// <summary>
        /// Forgot Password View  
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Forgot()
        {
            return View();
        }


        /// <summary>
        /// Forgot Password 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Forgot(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                // Child account cannot forget password

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email");
                    return View();
                }

                // Check if account is deleted                
                //if (user.IsDeleted != null)
                //{
                //    if (user.IsDeleted == true)
                //    {
                //        ModelState.AddModelError(string.Empty, "Invalid email");
                //        return View(model);
                //    }
                //}

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var mainLogo = $"{Convert.ToString(_configuration["WebsiteUrl:url"])}assets/images/brand-logos/logo.png";
                    var passwordResetLink = Url.ResetPasswordCallbackLink(user.Id, token, Request.Scheme);
                    var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
                    // Email template                        
                    var tbl = new Hashtable
                            {
                                { "@FirstName", user.FirstName },
                                { "@LastName", user.LastName },
                                { "@passwordResetLink", passwordResetLink },
                                { "@mainlogo",  mainLogo},
                                 { "@baseurl",  baseUrl}
                            };

                    string bodyContent = _emailService.GetMailBody("ForgotPassword.html", tbl);
                    EmailViewModel emailVm = new EmailViewModel { To = user.Email, Subject = "Password Reset Link", Body = bodyContent };

                    await _emailService.SendEmailAsync(emailVm);

                    // Log 
                    await _auditLogService.AuditLogsAsync(user.Id, "Reset password Link send successfully.");
                }

                return View("ForgotPasswordConfirmation");

            }
            return View(model);
        }


        /// <summary>
        /// reset password 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string Id)
        {
            if (token == null || Id == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset token");
            }

            return View();
        }


        /// <summary>
        /// Un Authorized 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Under Construction
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult UnderConstruction()
        {
            return View();
        }

        /// <summary>
        /// For Reset the Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
                        var mainLogo = $"{baseUrl}assets/images/brand-logos/logo.png";

                        // Email template                        
                        var tbl = new Hashtable
                            {
                                { "@FirstName", user.FirstName },
                                { "@LastName", user.LastName },
                                { "@baseurl", baseUrl },
                                { "@mainlogo",  mainLogo}
                            };

                        string bodyContent = _emailService.GetMailBody("ChangedPassword.html", tbl);

                        //Send Email to Succefully changed password
                        EmailViewModel emailModel = new EmailViewModel { To = user.Email, Subject = $"Password reset successfully", Body = bodyContent };
                        await _emailService.SendEmailAsync(emailModel);

                        // Log 
                        await _auditLogService.AuditLogsAsync(user.Id, "Password reset successfully.");

                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid user");
                    return View(model);
                }


            }

            return View(model);
        }

        ///<summary>
        /// Confirm Email
        /// </summary> 
        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if (Id == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user ID {Id} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [NonAction]
        private async Task<CBSUser> GetCurrentUserAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return await _userManager.GetUserAsync(HttpContext.User);

            return null;
        }




        [AllowAnonymous]
        public IActionResult Jwks()
        {
            var rsa = RSA.Create(2048);
            var jsonWebKey = new JsonWebKey
            {
                Kty = "RSA",
                Use = "sig",
                Kid = Guid.NewGuid().ToString(),
                E = Base64UrlEncoder.Encode(rsa.ExportParameters(false).Exponent),
                N = Base64UrlEncoder.Encode(rsa.ExportParameters(false).Modulus)
            };

            var jwks = new { keys = new[] { jsonWebKey } };

            return Ok(jwks);
        }
    }


}

