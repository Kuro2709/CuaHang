using System.Web;
using System.Web.Mvc;

namespace CuaHang.Attributes
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session;
            if (session["JWToken"] == null)
            {
                filterContext.Result = new RedirectResult("~/DangNhap/Index");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
