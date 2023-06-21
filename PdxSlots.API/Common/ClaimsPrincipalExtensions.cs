using PdxSlots.API.Common;
using System.Security.Claims;

namespace PdxSlots.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }  
        
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirst(Constants.Firebase_Claims_IsAdmin)?.Value == "true";
        }
    }
}
