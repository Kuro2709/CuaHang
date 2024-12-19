using CuaHang.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuaHang.Services
{
    public interface ISanPhamService
    {
        Task<IEnumerable<ThongTinSanPham>> GetProductsAsync();
        Task<ThongTinSanPham> GetProductAsync(string id);
        Task<bool> AddProductAsync(ThongTinSanPham product);
        Task<bool> EditProductAsync(string id, ThongTinSanPham product);
        Task<bool> DeleteProductAsync(string productId);
        Task<bool> ProductHasInvoicesAsync(string productId);
    }
}
