using System.ComponentModel.DataAnnotations;

namespace CuaHang.Models
{
    public class ThongTinSanPham
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string ProductID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid price")]
        public decimal Price { get; set; }
    }
}

