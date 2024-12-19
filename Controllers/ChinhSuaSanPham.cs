using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CuaHang.Models;
using Newtonsoft.Json;

namespace CuaHang.Controllers
{
    public class ChinhSuaSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChinhSuaSanPhamController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditProduct
        public async Task<ActionResult> Index(string id)
        {
            ThongTinSanPham productInfo = new ThongTinSanPham();
            try
            {
                var response = await _httpClient.GetAsync($"SanPham/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    productInfo = JsonConvert.DeserializeObject<ThongTinSanPham>(jsonString);
                }
                else
                {
                    errorMessage = "Không tìm thấy sản phẩm.";
                    ViewBag.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            return View(productInfo);
        }

        // POST: EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ThongTinSanPham productInfo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(productInfo.ProductID) || string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
                {
                    errorMessage = "All the fields are required";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                try
                {
                    var jsonContent = JsonConvert.SerializeObject(productInfo);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"SanPham/{productInfo.ProductID}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        successMessage = "Product updated successfully";
                        ViewBag.SuccessMessage = successMessage;
                        return RedirectToAction("Index", "SanPham");
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
