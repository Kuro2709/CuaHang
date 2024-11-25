using System.ComponentModel.DataAnnotations;

namespace CuaHang.Models
{
    public class CustomerInfo
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
        public string Phone { get; set; }
    }
}