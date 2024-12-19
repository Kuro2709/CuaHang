using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class XoaSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;

        public XoaSanPhamController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteProduct
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                // Send DELETE request to the API
                HttpResponseMessage response = await _httpClient.DeleteAsync($"SanPham/{id}");

                if (response.IsSuccessStatusCode)
                {
                    successMessage = "Sản phẩm đã được xóa thành công";
                    ViewBag.SuccessMessage = successMessage;
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            return RedirectToAction("Index", "SanPham");
        }
    }
}
