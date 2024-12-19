using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CuaHang.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;

namespace CuaHang.Services
{
    public class HoaDonApiService : IHoaDonService
    {
        private readonly HttpClient _httpClient;

        public HoaDonApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ThongTinHoaDon>> GetInvoicesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("HoaDon");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var invoiceDtos = JsonConvert.DeserializeObject<IEnumerable<ThongTinHoaDon>>(content);

                return invoiceDtos.Select(dto => new ThongTinHoaDon
                {
                    InvoiceID = dto.InvoiceID,
                    CustomerID = dto.CustomerID,
                    InvoiceDate = dto.InvoiceDate,
                    TotalPrice = dto.TotalPrice,
                    Customer = new ThongTinKhachHang
                    {
                        CustomerID = dto.Customer.CustomerID,
                        CustomerName = dto.Customer.CustomerName,
                        Phone = dto.Customer.Phone
                    },
                    InvoiceDetails = dto.InvoiceDetails.Select(detailDto => new ThongTinChiTietHoaDon
                    {
                        InvoiceDetailID = detailDto.InvoiceDetailID,
                        ProductID = detailDto.ProductID,
                        Quantity = detailDto.Quantity,
                        TotalPrice = detailDto.TotalPrice,
                        Product = new ThongTinSanPham
                        {
                            ProductID = detailDto.Product.ProductID,
                            ProductName = detailDto.Product.ProductName,
                            Price = detailDto.Product.Price
                        }
                    }).ToList()
                }).ToList();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }

        public async Task<ThongTinHoaDon> GetInvoiceByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"HoaDon/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var invoiceDto = JsonConvert.DeserializeObject<ThongTinHoaDon>(content);

                return new ThongTinHoaDon
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
                    InvoiceDetails = invoiceDto.InvoiceDetails.Select(detailDto => new ThongTinChiTietHoaDon
                    {
                        InvoiceDetailID = detailDto.InvoiceDetailID,
                        ProductID = detailDto.ProductID,
                        Quantity = detailDto.Quantity,
                        TotalPrice = detailDto.TotalPrice,
                        Product = new ThongTinSanPham
                        {
                            ProductID = detailDto.Product.ProductID,
                            ProductName = detailDto.Product.ProductName,
                            Price = detailDto.Product.Price
                        }
                    }).ToList()
                };
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddInvoiceAsync(ThongTinHoaDon invoice)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(invoice);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("HoaDon", content);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateInvoiceAsync(string id, ThongTinHoaDon invoice)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(invoice);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"HoaDon/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"HoaDon/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }
    }
}
