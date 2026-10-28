using Microsoft.AspNetCore.Http;

namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public static class IpHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetClientIp()
        {
            var clientIp = _httpContextAccessor.HttpContext?.Items.TryGetValue("ClientIp", out var ip) == true
                ? ip as string
                : null;
            return clientIp;
        }
    }
}
