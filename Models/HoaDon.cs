using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public void RecalculateTotalPrice(Func<string, decimal> getProductPrice)
        {
            TotalPrice = 0;
            foreach (var detail in InvoiceDetails)
            {
                detail.TotalPrice = detail.Quantity * getProductPrice(detail.ProductID);
                TotalPrice += detail.TotalPrice;
            }
        }
    }
}
