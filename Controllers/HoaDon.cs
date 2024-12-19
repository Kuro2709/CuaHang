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
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService ?? throw new ArgumentNullException(nameof(hoaDonService));
        }

        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetInvoices([DataSourceRequest] DataSourceRequest request)
        {
            List<ThongTinHoaDon> invoices = new List<ThongTinHoaDon>();

            try
            {
                var invoiceList = await _hoaDonService.GetInvoicesAsync();
                invoices = new List<ThongTinHoaDon>(invoiceList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return Json(invoices.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteInvoice([DataSourceRequest] DataSourceRequest request, ThongTinHoaDon invoice)
        {
            if (invoice != null && ModelState.IsValid)
            {
                try
                {
                    var result = await _hoaDonService.DeleteInvoiceAsync(invoice.InvoiceID);
                    if (!result)
                    {
                        ModelState.AddModelError("", "Error deleting invoice");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }
    }
}
