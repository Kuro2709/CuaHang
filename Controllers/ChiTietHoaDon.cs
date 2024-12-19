using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using CuaHang.Models;
using Newtonsoft.Json;

namespace CuaHang.Controllers
{
    public class ChiTietHoaDonController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChiTietHoaDonController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string errorMessage = "";
        public string successMessage = "";

        // GET: InvoiceDetails
        public async Task<ActionResult> Index(string id)
        {
            ThongTinHoaDon invoice = null;
            List<ThongTinChiTietHoaDon> invoiceDetails = new List<ThongTinChiTietHoaDon>();
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
                    errorMessage = "Error: Unable to retrieve invoice details.";
                    ViewBag.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            ViewBag.Invoice = invoice;
            return View(invoice?.InvoiceDetails ?? invoiceDetails);
        }
    }
}