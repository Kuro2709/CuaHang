using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class XoaKhachHangController : Controller
    {
        private readonly HttpClient _httpClient;

        public XoaKhachHangController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteCustomer
        public async Task<ActionResult> Index(string id)
        {
            try
            {
                var token = Session["JWTToken"]?.ToString();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "TrangChu");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Send DELETE request to the API
                HttpResponseMessage response = await _httpClient.DeleteAsync($"KhachHang/{id}");

                if (response.IsSuccessStatusCode)
                {
                    successMessage = "Khách hàng đã được xóa thành công";
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

            return RedirectToAction("Index", "KhachHang");
        }
    }
}
