using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Data.SqlClient;
using CuaHang.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CuaHang.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetInvoices([DataSourceRequest] DataSourceRequest request)
        {
            List<ThongTinHoaDon> invoices = new List<ThongTinHoaDon>();
            try
            {
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
                Console.WriteLine("Exception: " + ex.Message);
            }

            return Json(invoices.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
