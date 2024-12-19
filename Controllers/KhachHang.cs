using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using CuaHang.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using CuaHang.Services;

namespace CuaHang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly IKhachHangService _khachHangService;

        public KhachHangController(IKhachHangService khachHangService)
        {
            _khachHangService = khachHangService ?? throw new ArgumentNullException(nameof(khachHangService));
        }

        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetCustomers([DataSourceRequest] DataSourceRequest request)
        {
            List<ThongTinKhachHang> listCustomers = new List<ThongTinKhachHang>();

            try
            {
                var customers = await _khachHangService.GetCustomersAsync();
                listCustomers = new List<ThongTinKhachHang>(customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return Json(listCustomers.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCustomer([DataSourceRequest] DataSourceRequest request, ThongTinKhachHang customer)
        {
            if (customer != null)
            {
                try
                {
                    var success = await _khachHangService.DeleteCustomerAsync(customer.CustomerID);
                    if (!success)
                    {
                        ModelState.AddModelError("CustomerDeleteError", "Không thể xóa khách hàng. Khách hàng đang có trong một hoặc nhiều hóa đơn.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CustomerDeleteError", "Có lỗi xảy ra khi xóa khách hàng: " + ex.Message);
                }
            }

            return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        }
    }
}
