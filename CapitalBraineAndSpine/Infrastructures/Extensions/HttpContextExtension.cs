using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace HealthCare.Web.Infrastructures.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Get current logged in user id
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>int</returns>
        public static int GetCurrentUserId(this HttpContext httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var userIdClaim = httpContext.User.FindFirst("Id")?.Value;
                if (userIdClaim != null)
                    return Convert.ToInt32(userIdClaim);
            }
            return default;
        }

        /// <summary>
        /// Get current session value by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContext"></param>
        /// <param name="sessionKey"></param>
        /// <returns>T</returns>
        public static T GetSession<T>(this HttpContext httpContext, string sessionKey)
        {
            return JsonSerializer.Deserialize<T>(httpContext.Session.GetString(sessionKey));
        }
        /// <summary>
        /// Get Ip Address
        /// </summary>
        /// <param name="context"></param>
        /// <returns>string</returns>
        public static string GetClientIpAddress(this HttpContext context)
        {
            if (context != null && context.Connection != null)
                return Convert.ToString(context.Connection.RemoteIpAddress);

            return "Unknown";
        }

        /// <summary>
        /// Get current user name
        /// </summary>
        /// <param name="context"></param>
        /// <returns>string</returns>
        public static string GetUserName(this HttpContext context)
        {
            return context?.User?.Identity?.Name;
        }
    }

}
