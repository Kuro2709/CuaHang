using System.ComponentModel.DataAnnotations;

namespace CuaHang.Models
{
    public class ThongTinKhachHang
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string CustomerID { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa số và không có khoảng trắng")]
        public string Phone { get; set; }
    }
}
