using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHang.Models
{
    public class ThongTinChiTietHoaDon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceDetailID { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceID { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng nhập số lượng hợp lệ")]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        [ForeignKey("ProductID")]
        public virtual ThongTinSanPham Product { get; set; }
    }
}
