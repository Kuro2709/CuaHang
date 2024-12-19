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
    public class ChinhSuaHoaDonController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChinhSuaHoaDonController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7262/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: EditInvoice
        public async Task<ActionResult> Index(string id)
        {
            ThongTinHoaDon invoice = null;
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"HoaDon/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var invoiceDto = JsonConvert.DeserializeObject<HoaDonDto>(jsonString);

                    invoice = new ThongTinHoaDon
                    {
                        InvoiceID = invoiceDto.InvoiceID,
                        CustomerID = invoiceDto.CustomerID,
                        InvoiceDate = invoiceDto.InvoiceDate,
                        TotalPrice = invoiceDto.TotalPrice,
                        Customer = new ThongTinKhachHang
                        {
                            CustomerID = invoiceDto.Customer.CustomerID,
                            CustomerName = invoiceDto.Customer.CustomerName,
                            Phone = invoiceDto.Customer.Phone
                        },
                        InvoiceDetails = new List<ThongTinChiTietHoaDon>()
                    };

                    foreach (var detailDto in invoiceDto.ChiTietHoaDon)
                    {
                        var detail = new ThongTinChiTietHoaDon
                        {
                            InvoiceDetailID = detailDto.InvoiceDetailID,
                            ProductID = detailDto.ProductID,
                            Product = new ThongTinSanPham
                            {
                                ProductID = detailDto.Product.ProductID,
                                ProductName = detailDto.Product.ProductName,
                                Price = detailDto.Product.Price
                            },
                            Quantity = detailDto.Quantity,
                            TotalPrice = detailDto.TotalPrice
                        };
                        invoice.InvoiceDetails.Add(detail);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Error: Unable to retrieve invoice details.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Exception: " + ex.Message;
            }

            ViewBag.Customers = await GetCustomers();
            ViewBag.Products = await GetProducts();
            return View(invoice);
        }

        // POST: EditInvoice
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
            invoice.InvoiceID = invoice.InvoiceID?.Trim().ToUpper();

            // Populate InvoiceDetails from form data
            invoice.InvoiceDetails = new List<ThongTinChiTietHoaDon>();
            for (int i = 0; i < ProductID.Length; i++)
            {
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

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate total price
                    await invoice.RecalculateTotalPrice(productID => GetProductPrice(productID));

                    // Update invoice and invoice details
                    await UpdateInvoice(invoice);
                    TempData["SuccessMessage"] = "Hóa đơn đã được cập nhật thành công.";
                    return RedirectToAction("Index", new { id = invoice.InvoiceID });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            ViewBag.Customers = await GetCustomers();
            ViewBag.Products = await GetProducts();
            return View(invoice);
        }

        private async Task UpdateInvoice(ThongTinHoaDon invoice)
        {
            try
            {
                // Serialize the invoice object to JSON
                var jsonPayload = JsonConvert.SerializeObject(invoice);

                // Log the JSON payload
                System.Diagnostics.Debug.WriteLine("PUT JSON Payload: " + jsonPayload);

                // Create the HTTP content with the JSON payload
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                // Send the PUT request
                var response = await _httpClient.PutAsync($"HoaDon/{invoice.InvoiceID}", content);

                // Log the response content for debugging purposes
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("API Response Content: " + responseContent);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HttpRequestException: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
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

        [HttpPost]
        public async Task<ActionResult> AddInvoiceDetail()
        {
            ViewBag.Products = await GetProducts();
            return PartialView("_InvoiceDetailRow", new ThongTinChiTietHoaDon());
        }

        [HttpGet]
        public async Task<JsonResult> GetProductPriceByID(string productID)
        {
            decimal price = await GetProductPrice(productID);
            return Json(price, JsonRequestBehavior.AllowGet);
        }
    }
}
