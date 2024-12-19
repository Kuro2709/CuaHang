using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CuaHang.Models;
using Newtonsoft.Json;

namespace CuaHang.Controllers
{
    public class ChinhSuaKhachHangController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChinhSuaKhachHangController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditCustomer
        public async Task<ActionResult> Index(string id)
        {
            ThongTinKhachHang customerInfo = new ThongTinKhachHang();
            try
            {
                var response = await _httpClient.GetAsync($"KhachHang/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    customerInfo = JsonConvert.DeserializeObject<ThongTinKhachHang>(jsonString);
                }
                else
                {
                    errorMessage = "Không tìm thấy khách hàng.";
                    ViewBag.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            return View(customerInfo);
        }

        // POST: EditCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ThongTinKhachHang customerInfo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(customerInfo.CustomerID) || string.IsNullOrEmpty(customerInfo.CustomerName) || string.IsNullOrEmpty(customerInfo.Phone))
                {
                    errorMessage = "All the fields are required";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                try
                {
                    var jsonContent = JsonConvert.SerializeObject(customerInfo);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"KhachHang/{customerInfo.CustomerID}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        successMessage = "Chỉnh sửa khách hàng thành công";
                        ViewBag.SuccessMessage = successMessage;
                        return RedirectToAction("Index", "KhachHang");
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = errorMessage;
                        return View(customerInfo);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }
            }

            return View(customerInfo);
        }
    }
}

