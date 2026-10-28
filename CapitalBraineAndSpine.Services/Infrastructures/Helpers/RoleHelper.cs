using System.Security.Claims;

namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public class RoleHelper
    {
        public static bool UserHasAnyRole(ClaimsPrincipal user, params string[] roles)
        {
            foreach (var role in roles)
            {
                if (user.Claims.Any(c => c.Type == ClaimTypes.Role && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
