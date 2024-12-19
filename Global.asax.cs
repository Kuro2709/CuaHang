using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using CuaHang.App_Start;
using Owin;
using System.Web;
using System.Web.SessionState;

namespace CuaHang
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();

            // Register Web API configuration
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }

        public void Configuration(IAppBuilder app)
        {
            JwtConfig.ConfigureJwt(app);
        }
    }
}
