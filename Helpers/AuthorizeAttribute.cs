using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVCData.Helpers
{
    /// <summary>
    /// Attribute để kiểm tra xem user đã đăng nhập hay chưa
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public AuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserID");
            
            if (userId == null)
            {
                // User chưa đăng nhập
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Nếu có yêu cầu về role
            if (_allowedRoles.Length > 0)
            {
                var userRole = context.HttpContext.Session.GetString("Role");
                
                if (string.IsNullOrEmpty(userRole) || !_allowedRoles.Contains(userRole))
                {
                    // User không có quyền
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Attribute để kiểm tra role cụ thể
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
