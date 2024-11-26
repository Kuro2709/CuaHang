using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class HoaDonController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: Invoice
        public ActionResult Index()
        {
            List<ThongTinHoaDon> invoices = new List<ThongTinHoaDon>();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT i.InvoiceID, c.CustomerName, i.TotalPrice FROM Invoice i JOIN Customer c ON i.CustomerID = c.CustomerID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThongTinHoaDon invoice = new ThongTinHoaDon
                                {
                                    InvoiceID = reader["InvoiceID"].ToString(),
                                    Customer = new ThongTinKhachHang { CustomerName = reader["CustomerName"].ToString() },
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                                };
                                invoices.Add(invoice);
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

            return View(invoices);
        }
    }
}








