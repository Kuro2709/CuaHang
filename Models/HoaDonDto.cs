using System.Collections.Generic;
using System;

namespace CuaHang.Models
{
    public class HoaDonDto
    {
        public string InvoiceID { get; set; }
        public string CustomerID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalPrice { get; set; }
        public KhachHangDto Customer { get; set; }
        public List<ChiTietHoaDonDto> ChiTietHoaDon { get; set; }
    }

    public class KhachHangDto
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
    }

    public class ChiTietHoaDonDto
    {
        public int InvoiceDetailID { get; set; }
        public string InvoiceID { get; set; }
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public SanPhamDto Product { get; set; }
    }

    public class SanPhamDto
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
