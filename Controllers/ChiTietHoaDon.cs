using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ChiTietHoaDonController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: InvoiceDetails
        public ActionResult Index(string id)
        {
            ThongTinHoaDon invoice = new ThongTinHoaDon();
            List<ThongTinChiTietHoaDon> invoiceDetails = new List<ThongTinChiTietHoaDon>();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Invoice WHERE InvoiceID = @InvoiceID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                invoice.InvoiceID = reader["InvoiceID"].ToString();
                                invoice.CustomerID = reader["CustomerID"].ToString();
                                invoice.InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]);
                                invoice.TotalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                            }
                        }
                    }

                    sql = "SELECT d.InvoiceDetailID, d.ProductID, p.ProductName, d.Quantity, d.TotalPrice FROM InvoiceDetails d JOIN Product p ON d.ProductID = p.ProductID WHERE d.InvoiceID = @InvoiceID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThongTinChiTietHoaDon detail = new ThongTinChiTietHoaDon
                                {
                                    InvoiceDetailID = Convert.ToInt32(reader["InvoiceDetailID"]),
                                    ProductID = reader["ProductID"].ToString(),
                                    Product = new ThongTinSanPham { ProductName = reader["ProductName"].ToString() },
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                                };
                                invoiceDetails.Add(detail);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            ViewBag.Invoice = invoice;
            return View(invoiceDetails);
        }
    }
}
