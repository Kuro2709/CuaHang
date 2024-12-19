using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class XoaHoaDonController : Controller
    {
        private readonly HttpClient _httpClient;

        public XoaHoaDonController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteInvoice
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                // Send DELETE request to the API
                HttpResponseMessage response = await _httpClient.DeleteAsync($"HoaDon/{id}");

                if (response.IsSuccessStatusCode)
                {
                    successMessage = "Hóa đơn đã được xóa thành công";
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

            return RedirectToAction("Index", "HoaDon");
        }
    }
}
