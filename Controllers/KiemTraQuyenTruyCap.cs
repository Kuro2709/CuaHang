using System.Web;
using System.Web.Mvc;

namespace CuaHang.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session;
            if (session["JWTToken"] == null)
            {
                filterContext.Result = new RedirectResult("~/DangNhap/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}

