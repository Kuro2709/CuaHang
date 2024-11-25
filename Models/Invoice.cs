using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CuaHang.Models
{
    public class InvoiceInfo
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
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid total price")]
        public decimal TotalPrice { get; set; }

        public virtual CustomerInfo Customer { get; set; }

        public virtual ICollection<InvoiceDetailsInfo> InvoiceDetails { get; set; } = new List<InvoiceDetailsInfo>();

        public void RecalculateTotalPrice(Func<string, decimal> getProductPrice)
        {
            TotalPrice = InvoiceDetails.Sum(d => d.Quantity * getProductPrice(d.ProductID));
        }
    }

}







