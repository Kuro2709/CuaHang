using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CuaHang.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CuaHang.Services
{
    public class KhachHangApiService : IKhachHangService
    {
        private readonly HttpClient _httpClient;

        public KhachHangApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ThongTinKhachHang>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("KhachHang");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<ThongTinKhachHang>>(content);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"KhachHang/{customerId}");
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