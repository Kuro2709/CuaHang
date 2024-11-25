using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class InvoiceController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: Invoice
        public ActionResult Index()
        {
            List<InvoiceInfo> invoices = new List<InvoiceInfo>();
            try
            {
                string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
                                InvoiceInfo invoice = new InvoiceInfo
                                {
                                    InvoiceID = reader["InvoiceID"].ToString(),
                                    Customer = new CustomerInfo { CustomerName = reader["CustomerName"].ToString() },
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








