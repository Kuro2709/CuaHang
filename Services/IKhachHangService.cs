using CuaHang.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuaHang.Services
{
    public interface IKhachHangService
    {
        Task<IEnumerable<ThongTinKhachHang>> GetCustomersAsync();
        Task<bool> DeleteCustomerAsync(string customerId);
    }
}