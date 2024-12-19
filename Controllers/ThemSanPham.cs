using System;
using System.Web.Mvc;
using CuaHang.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CuaHang.Controllers
{
    public class ThemSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;

        public ThemSanPhamController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: ThemSanPham
        public ActionResult Index()
        {
            return View(new ThongTinSanPham());
        }

        // POST: ThemSanPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ThongTinSanPham productInfo)
        {
            if (ModelState.IsValid)
            {
                // Trim spaces and convert ProductID to uppercase
                productInfo.ProductID = productInfo.ProductID?.Trim().ToUpper();

                if (string.IsNullOrEmpty(productInfo.ProductID) || string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
                {
                    errorMessage = "Tất cả các mục đều bắt buộc, không được để trống";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                if (productInfo.ProductID.Contains(" "))
                {
                    errorMessage = "Mã sản phẩm không được chứa khoảng trắng";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                try
                {
                    var jsonContent = JsonConvert.SerializeObject(productInfo);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("SanPham", content);
                    if (response.IsSuccessStatusCode)
                    {
                        successMessage = "Sản phẩm đã được thêm thành công";
                        ViewBag.SuccessMessage = successMessage;
                        ModelState.Clear();
                        return View(new ThongTinSanPham());
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = errorMessage;
                        return View(productInfo);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }
            }

            return View(productInfo);
        }
    }
}
