using kursah_5semestr;
using kursah_5semestr.Abstractions;
using System.Security.Claims;

namespace kursah_5semestr.Controllers
{
    public class Utils
    {
        public static User? GetAuthenticatedUser(HttpContext httpContext, IUsersService usersService)
        {
            if (httpContext.User.Identity is ClaimsIdentity identity)
            {
                var login = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
                if (!string.IsNullOrEmpty(login))
                {
                    return usersService.GetUserByLogin(login);
                }
            }
            return null;
        }
    }
}
