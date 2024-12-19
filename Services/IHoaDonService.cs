using CuaHang.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuaHang.Services
{
    public interface IHoaDonService
    {
        Task<IEnumerable<ThongTinHoaDon>> GetInvoicesAsync();
        Task<ThongTinHoaDon> GetInvoiceByIdAsync(string id);
        Task<bool> AddInvoiceAsync(ThongTinHoaDon invoice);
        Task<bool> UpdateInvoiceAsync(string id, ThongTinHoaDon invoice);
        Task<bool> DeleteInvoiceAsync(string id);
    }
}
