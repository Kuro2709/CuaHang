using System;
using System.Web.Mvc;
using CuaHang.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CuaHang.Controllers
{
    public class ThemKhachHangController : Controller
    {
        private readonly HttpClient _httpClient;

        public ThemKhachHangController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: ThemKhachHang
        public ActionResult Index()
        {
            return View(new ThongTinKhachHang());
        }

        // POST: ThemKhachHang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ThongTinKhachHang customerInfo)
        {
            if (ModelState.IsValid)
            {
                // Trim spaces and convert CustomerID to uppercase
                customerInfo.CustomerID = customerInfo.CustomerID?.Trim().ToUpper();

                if (string.IsNullOrEmpty(customerInfo.CustomerID) || string.IsNullOrEmpty(customerInfo.CustomerName) || string.IsNullOrEmpty(customerInfo.Phone))
                {
                    errorMessage = "Tất cả các mục đều bắt buộc, không được để trống";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                if (customerInfo.CustomerID.Contains(" "))
                {
                    errorMessage = "Mã khách hàng không được chứa khoảng trắng";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(customerInfo.Phone, @"^\d+$"))
                {
                    errorMessage = "Số điện thoại chỉ được chứa số và không có khoảng trắng";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                try
                { 
                    var jsonContent = JsonConvert.SerializeObject(customerInfo);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("KhachHang", content);
                    if (response.IsSuccessStatusCode)
                    {
                        successMessage = "Khách hàng đã được thêm thành công";
                        ViewBag.SuccessMessage = successMessage;
                        ModelState.Clear();
                        return View(new ThongTinKhachHang());
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
