using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using CuaHang.Models;
using Newtonsoft.Json;

namespace CuaHang.Controllers
{
    public class ThemHoaDonController : Controller
    {
        private readonly HttpClient _httpClient;

        public ThemHoaDonController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7262/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: ThemHoaDon
        public async Task<ActionResult> Index()
        {
            ViewBag.Customers = await GetCustomers();
            ViewBag.Products = await GetProducts();
            return View(new ThongTinHoaDon { InvoiceDetails = new List<ThongTinChiTietHoaDon> { new ThongTinChiTietHoaDon() } });
        }

        // POST: ThemHoaDon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ThongTinHoaDon invoice, string[] ProductID, int[] Quantity)
        {
            

            // Log form data
            foreach (var key in Request.Form.AllKeys)
            {
                System.Diagnostics.Debug.WriteLine($"{key}: {Request.Form[key]}");
            }

            // Trim spaces and convert InvoiceID to uppercase
            invoice.InvoiceID = invoice.InvoiceID?.Trim().ToUpper().Replace(" ", "");
            System.Diagnostics.Debug.WriteLine($"Trimmed and uppercased InvoiceID: {invoice.InvoiceID}");

            // Populate InvoiceDetails from form data
            invoice.InvoiceDetails = new List<ThongTinChiTietHoaDon>();
            for (int i = 0; i < ProductID.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Processing ProductID: {ProductID[i]} with Quantity: {Quantity[i]}");

                var detail = new ThongTinChiTietHoaDon
                {
                    InvoiceID = invoice.InvoiceID,
                    ProductID = ProductID[i],
                    Quantity = Quantity[i],
                    TotalPrice = Quantity[i] * await GetProductPrice(ProductID[i]),
                    Product = await GetProductById(ProductID[i])
                };
                invoice.InvoiceDetails.Add(detail);
            }

            System.Diagnostics.Debug.WriteLine("Invoice details populated.");

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for duplicate InvoiceID
                    System.Diagnostics.Debug.WriteLine("Checking for duplicate InvoiceID.");
                    if (await InvoiceExists(invoice.InvoiceID))
                    {
                        ModelState.AddModelError("InvoiceID", "Mã hóa đơn đã tồn tại.");
                        System.Diagnostics.Debug.WriteLine("Duplicate InvoiceID found.");
                    }
                    else if (invoice.InvoiceDetails == null || invoice.InvoiceDetails.Count == 0)
                    {
                        ModelState.AddModelError("", "Bạn phải thêm ít nhất một dòng mặt hàng.");
                        System.Diagnostics.Debug.WriteLine("No invoice details found.");
                    }
                    else
                    {
                        // Calculate total price
                        System.Diagnostics.Debug.WriteLine("Recalculating total price.");
                        await invoice.RecalculateTotalPrice(productID => GetProductPrice(productID));
                        System.Diagnostics.Debug.WriteLine("Total price recalculated.");

                        // Insert invoice and invoice details
                        System.Diagnostics.Debug.WriteLine("Inserting invoice.");
                        await InsertInvoice(invoice);
                        System.Diagnostics.Debug.WriteLine("Invoice inserted successfully.");
                        TempData["SuccessMessage"] = "Hóa đơn đã được thêm thành công.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            ViewBag.Customers = await GetCustomers();
            ViewBag.Products = await GetProducts();
            System.Diagnostics.Debug.WriteLine("Returning view with invoice data.");
            return View(invoice);
        }

        [HttpPost]
        public async Task<ActionResult> AddInvoiceDetail()
        {
            ViewBag.Products = await GetProducts();
            return PartialView("_InvoiceDetailRow", new ThongTinChiTietHoaDon());
        }

        private async Task<bool> InvoiceExists(string invoiceID)
        {
            var response = await _httpClient.GetAsync($"HoaDon/{invoiceID}");
            return response.IsSuccessStatusCode;
        }

        private async Task InsertInvoice(ThongTinHoaDon invoice)
        {
            System.Diagnostics.Debug.WriteLine("Starting InsertInvoice method.");
            var content = new StringContent(JsonConvert.SerializeObject(invoice), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("HoaDon", content);
            response.EnsureSuccessStatusCode();
            System.Diagnostics.Debug.WriteLine("InsertInvoice method completed successfully.");
        }

        [HttpGet]
        public async Task<JsonResult> GetProductPriceByID(string productID)
        {
            decimal price = await GetProductPrice(productID);
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        private async Task<decimal> GetProductPrice(string productID)
        {
            var response = await _httpClient.GetAsync($"HoaDon/ProductPrice/{productID}");
            response.EnsureSuccessStatusCode();
            var price = JsonConvert.DeserializeObject<decimal>(await response.Content.ReadAsStringAsync());
            return price;
        }

        private async Task<ThongTinSanPham> GetProductById(string productId)
        {
            var response = await _httpClient.GetAsync($"HoaDon/Products/{productId}");
            response.EnsureSuccessStatusCode();
            var product = JsonConvert.DeserializeObject<ThongTinSanPham>(await response.Content.ReadAsStringAsync());
            return product;
        }

        private async Task<SelectList> GetCustomers()
        {
            var response = await _httpClient.GetAsync("HoaDon/Customers");
            response.EnsureSuccessStatusCode();
            var customers = JsonConvert.DeserializeObject<List<ThongTinKhachHang>>(await response.Content.ReadAsStringAsync());
            return new SelectList(customers, "CustomerID", "CustomerName");
        }

        private async Task<SelectList> GetProducts()
        {
            var response = await _httpClient.GetAsync("HoaDon/Products");
            response.EnsureSuccessStatusCode();
            var products = JsonConvert.DeserializeObject<List<ThongTinSanPham>>(await response.Content.ReadAsStringAsync());
            return new SelectList(products, "ProductID", "ProductName");
        }
    }
}
