using System.Linq;
using System.Web.Mvc;
using Unity.AspNet.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CuaHang.App_Start.UnityMvcActivator), nameof(CuaHang.App_Start.UnityMvcActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(CuaHang.App_Start.UnityMvcActivator), nameof(CuaHang.App_Start.UnityMvcActivator.Shutdown))]

namespace CuaHang.App_Start
{
    public static class UnityMvcActivator
    {
        public static void Start()
        {
            var container = UnityConfig.RegisterComponents();
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        public static void Shutdown()
        {
            var container = UnityConfig.Container;
            container.Dispose();
        }
    }
}
