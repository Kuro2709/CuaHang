using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CuaHang.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CuaHang.Services
{
    public class SanPhamApiService : ISanPhamService
    {
        private readonly HttpClient _httpClient;

        public SanPhamApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
        }

        public async Task<IEnumerable<ThongTinSanPham>> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("SanPham");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<ThongTinSanPham>>(content);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<ThongTinSanPham> GetProductAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SanPham/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ThongTinSanPham>(content);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> AddProductAsync(ThongTinSanPham product)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("SanPham", content);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> EditProductAsync(string id, ThongTinSanPham product)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"SanPham/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SanPham/{productId}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> ProductHasInvoicesAsync(string productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SanPham/ProductHasInvoices/{productId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(content);
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
