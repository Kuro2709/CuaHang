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
        [Range(typeof(decimal), "0", "100000000", ErrorMessage = "Giá trị quá lớn hoặc quá nhỏ, vui lòng nhập giá trị hợp lệ")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Giá thành chỉ được chứa số và tối đa hai chữ số thập phân")]
        public decimal Price { get; set; }
    }
}
