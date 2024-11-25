using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHang.Models
{
    public class InvoiceDetailsInfo
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
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid quantity")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid total price")]
        public decimal TotalPrice { get; set; }

        public virtual InvoiceInfo Invoice { get; set; }
        public virtual ProductInfo Product { get; set; }
    }
}







