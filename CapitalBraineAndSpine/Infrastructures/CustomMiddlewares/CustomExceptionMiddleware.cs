using HealthCare.Data.Contexts;
using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Data.Entities.LogsEntities;
using HealthCare.Service.Infrastructures.Extensions;
using HealthCare.Service.Infrastructures.Helpers;
using HealthCare.Service.Models.ApiResponse;
using HealthCare.Web.Infrastructures.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealthCare.Web.Infrastructures.CustomerMiddlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomExceptionMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            this.next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Invoke(HttpContext context, IServiceScopeFactory serviceScopeFactory)
        {
            var IpAddress = string.Empty;
            try
            {
                var routeData = context.GetRouteData();
                string controller = null, action = null;
                if (routeData != null && routeData.Values.Count > 0)
                {
                    controller = routeData.Values["controller"]?.ToString();
                    action = routeData.Values["action"]?.ToString();
                }
                var user = _httpContextAccessor.HttpContext.User;
                string url = context.Request.GetEncodedUrl();
                string urlRef = context.Request.Headers["Referer"];
                string method = context.Request.Method;
                var userIdClaim = user.FindFirst(ClaimTypes.Sid);

                // Resolve DbContext from the request scope
                var dbContext = context.RequestServices.GetService<BaseDbContext>();
                if (dbContext != null)
                {
                    RequestLog requestLog = new RequestLog
                    {
                        Action = action,
                        Method = method,
                        UserId = userIdClaim?.Value,
                        Controller = controller,
                        ReferUrl = urlRef,
                        CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                        LastUpdated = CBSHelpers.CurrentLocalDateTime(),
                    };


                    IpAddress = context.GetClientIpAddress();
                    requestLog.IpAddress = IpAddress;
                    await dbContext.AddAsync(requestLog);
                    await dbContext.SaveChangesAsync();
                }
                await next(context);
            }
            catch (Exception exceptionObj)
            {
                await HandleExceptionAsync(context, exceptionObj, serviceScopeFactory, IpAddress);
            }
        }

        /// <summary>
        /// Custom exception handler 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <param name="serviceScopeFactory"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception, IServiceScopeFactory serviceScopeFactory, string IpAddress)
        {



            context.Response.ContentType = "application/json";
            var result = new ApiResponse { Status = false, Message = "Internal server error occurred.", Data = null };
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            using (var scope = serviceScopeFactory.CreateScope())
            {
                string currentUser = string.Empty;
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CBSUser>>();
                if (context.User.Identity.IsAuthenticated)
                {
                    var user = userManager.GetUserAsync(context.User).Result;
                    currentUser = user.UserName;
                }

                var exceptionDetails = exception.GetExceptionDetails();

                // Resolve DbContext from the request scope
                var dbContext = context.RequestServices.GetService<BaseDbContext>();
                if (dbContext != null)
                {
                    ExceptionLog exceptionLog = new ExceptionLog
                    {
                        UserId = currentUser,
                        LineNumber = exceptionDetails.LineNumber,
                        FileName = exceptionDetails.FileName,
                        Method = exceptionDetails.Method,
                        Class = exceptionDetails.Class,
                        ColumnNumber = exceptionDetails.ColumnNumber,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        InnerExMessage = exception.InnerException?.Message,
                        CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                        LastUpdated = CBSHelpers.CurrentLocalDateTime(),
                        IPAddress = IpAddress
                    };
                    dbContext.Add(exceptionLog);
                    dbContext.SaveChanges();
                }

            }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));


        }
    }
}
