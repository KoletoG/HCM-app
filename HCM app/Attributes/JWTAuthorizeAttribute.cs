using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HCM_app.Attributes
{
    public class JWTAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("jwt");
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("LoginMain", "Home", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
