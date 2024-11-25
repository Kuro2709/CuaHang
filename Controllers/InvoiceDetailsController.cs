using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class InvoiceDetailsController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: InvoiceDetails
        public ActionResult Index(string id)
        {
            InvoiceInfo invoice = new InvoiceInfo();
            List<InvoiceDetailsInfo> invoiceDetails = new List<InvoiceDetailsInfo>();
            try
            {
                string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
                                InvoiceDetailsInfo detail = new InvoiceDetailsInfo
                                {
                                    InvoiceDetailID = Convert.ToInt32(reader["InvoiceDetailID"]),
                                    ProductID = reader["ProductID"].ToString(),
                                    Product = new ProductInfo { ProductName = reader["ProductName"].ToString() },
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








