using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using CuaHang.Services;

namespace CuaHang.App_Start
{
    public static class UnityConfig
    {
        public static IUnityContainer Container { get; private set; }

        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();

            // Register your services here
            container.RegisterType<ISanPhamService, SanPhamApiService>();
            container.RegisterType<IKhachHangService, KhachHangApiService>();
            container.RegisterType<IHoaDonService, HoaDonApiService>();
            

            // Register HttpClient with SSL validation bypass for local development
            container.RegisterFactory<HttpClient>(c =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => true // Bypass SSL validation
                };

                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://localhost:7262/api/")
                };

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            });

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            Container = container;
            return container;
        }
    }
}
