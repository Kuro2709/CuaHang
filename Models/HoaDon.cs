using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CuaHang.Models
{
    public class ThongTinHoaDon
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string InvoiceID { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerID { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalPrice { get; set; }

        public virtual ThongTinKhachHang Customer { get; set; }
        public virtual ICollection<ThongTinChiTietHoaDon> InvoiceDetails { get; set; } = new List<ThongTinChiTietHoaDon>();

        public async Task RecalculateTotalPrice(Func<string, Task<decimal>> getProductPriceAsync)
        {
            System.Diagnostics.Debug.WriteLine("Starting RecalculateTotalPrice method.");
            TotalPrice = 0;
            foreach (var detail in InvoiceDetails)
            {
                try
                {
                    detail.TotalPrice = detail.Quantity * await getProductPriceAsync(detail.ProductID);
                    TotalPrice += detail.TotalPrice;
                    System.Diagnostics.Debug.WriteLine($"Detail: ProductID={detail.ProductID}, Quantity={detail.Quantity}, TotalPrice={detail.TotalPrice}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error calculating price for ProductID={detail.ProductID}: {ex.Message}");
                    throw;
                }
            }
            System.Diagnostics.Debug.WriteLine($"Recalculated TotalPrice: {TotalPrice}");
        }

    }
}
