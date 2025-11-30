using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagerAPI.Helpers
{
    public class AdminOnlyAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new { message = "You need to login first." }) { StatusCode = 401 };
                return;
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin")
            {
                context.Result = new JsonResult(new { message = "Access denied: only Admin can access this endpoint." }) { StatusCode = 403 };
            }
        }
    }
}
