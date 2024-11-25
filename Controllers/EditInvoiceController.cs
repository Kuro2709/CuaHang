using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class EditInvoiceController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditInvoice
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

                    sql = "SELECT d.InvoiceDetailID, d.ProductID, p.ProductName, d.Quantity, d.TotalPrice FROM InvoiceDetails d LEFT JOIN Product p ON d.ProductID = p.ProductID WHERE d.InvoiceID = @InvoiceID";
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
                return View(invoice);
            }

            ViewBag.InvoiceDetails = invoiceDetails;
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InvoiceInfo invoice, List<InvoiceDetailsInfo> invoiceDetails)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(invoice.CustomerID) || invoice.InvoiceDate == default || invoice.TotalPrice <= 0)
                {
                    errorMessage = "All the fields are required";
                    ViewBag.ErrorMessage = errorMessage;
                    ViewBag.InvoiceDetails = invoiceDetails;
                    return View(invoice);
                }

                try
                {
                    string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "UPDATE Invoice SET CustomerID = @CustomerID, InvoiceDate = @InvoiceDate, TotalPrice = @TotalPrice WHERE InvoiceID = @InvoiceID";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                            command.Parameters.AddWithValue("@CustomerID", invoice.CustomerID);
                            command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                            command.Parameters.AddWithValue("@TotalPrice", invoice.TotalPrice);
                            command.ExecuteNonQuery();
                        }

                        if (invoiceDetails == null)
                        {
                            invoiceDetails = new List<InvoiceDetailsInfo>();
                        }

                        foreach (var detail in invoiceDetails)
                        {
                            sql = "UPDATE InvoiceDetails SET ProductID = @ProductID, Quantity = @Quantity, TotalPrice = @TotalPrice WHERE InvoiceDetailID = @InvoiceDetailID";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@InvoiceDetailID", detail.InvoiceDetailID);
                                command.Parameters.AddWithValue("@ProductID", detail.ProductID);
                                command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                command.Parameters.AddWithValue("@TotalPrice", detail.TotalPrice);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    successMessage = "Invoice updated successfully";
                    ViewBag.SuccessMessage = successMessage;
                    return RedirectToAction("Index", "Invoice");
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    ViewBag.ErrorMessage = errorMessage;
                    ViewBag.InvoiceDetails = invoiceDetails;
                    return View(invoice);
                }
            }

            ViewBag.InvoiceDetails = invoiceDetails;
            return View(invoice);
        }

    }
}









