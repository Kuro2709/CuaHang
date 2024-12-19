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
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService ?? throw new ArgumentNullException(nameof(sanPhamService));
        }

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetProducts([DataSourceRequest] DataSourceRequest request)
        {
            List<ThongTinSanPham> listProducts = new List<ThongTinSanPham>();

            try
            {
                var products = await _sanPhamService.GetProductsAsync();
                listProducts = new List<ThongTinSanPham>(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return Json(listProducts.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProduct([DataSourceRequest] DataSourceRequest request, ThongTinSanPham product)
        {
            if (product != null)
            {
                if (await _sanPhamService.ProductHasInvoicesAsync(product.ProductID))
                {
                    ModelState.AddModelError("", "Không thể xóa sản phẩm. Sản phẩm đang có trong một hoặc nhiều hóa đơn.");
                }
                else
                {
                    try
                    {
                        var success = await _sanPhamService.DeleteProductAsync(product.ProductID);
                        if (!success)
                        {
                            ModelState.AddModelError("ProductDeleteError", "Không thể xóa sản phẩm. Sản phẩm đang có trong một hoặc nhiều hóa đơn.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ProductDeleteError", "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message);
                    }
                }
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }
    }
}
