using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class EditInvoiceController : Controller
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"; // Replace with your actual connection string

        // GET: EditInvoice
        public ActionResult Index(string id)
        {
            var invoice = GetInvoiceById(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }

            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            ViewBag.InvoiceDetails = invoice.InvoiceDetails;
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InvoiceInfo invoice)
        {
            // Trim spaces and convert InvoiceID to uppercase
            invoice.InvoiceID = invoice.InvoiceID?.Trim().ToUpper();

            if (ModelState.IsValid)
            {
                // Calculate total price
                invoice.RecalculateTotalPrice(GetProductPrice);

                // Update invoice and invoice details
                UpdateInvoice(invoice);
                TempData["SuccessMessage"] = "Invoice updated successfully.";
                return RedirectToAction("Index", new { id = invoice.InvoiceID });
            }

            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            ViewBag.InvoiceDetails = invoice.InvoiceDetails;
            return View(invoice);
        }

        [HttpPost]
        public ActionResult AddInvoiceDetail()
        {
            ViewBag.Products = GetProducts();
            return PartialView("_InvoiceDetailRow", new InvoiceDetailsInfo());
        }

        [HttpGet]
        public JsonResult GetProductPriceByID(string productID)
        {
            decimal price = GetProductPrice(productID);
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        private InvoiceInfo GetInvoiceById(string id)
        {
            // Implement the logic to get the invoice by ID from the database
            // This is just a placeholder implementation
            return new InvoiceInfo
            {
                InvoiceID = id,
                CustomerID = "C001",
                InvoiceDate = DateTime.Now,
                TotalPrice = 1000,
                InvoiceDetails = new List<InvoiceDetailsInfo>
                {
                    new InvoiceDetailsInfo
                    {
                        InvoiceDetailID = 1,
                        InvoiceID = id,
                        ProductID = "P001",
                        Quantity = 2,
                        TotalPrice = 200,
                        Product = new ProductInfo { ProductID = "P001", ProductName = "Product 1" }
                    }
                }
            };
        }

        private void UpdateInvoice(InvoiceInfo invoice)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update invoice
                        string updateInvoiceSql = "UPDATE Invoice SET CustomerID = @CustomerID, InvoiceDate = @InvoiceDate, TotalPrice = @TotalPrice WHERE InvoiceID = @InvoiceID";
                        using (SqlCommand command = new SqlCommand(updateInvoiceSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                            command.Parameters.AddWithValue("@CustomerID", invoice.CustomerID);
                            command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                            command.Parameters.AddWithValue("@TotalPrice", invoice.TotalPrice);
                            command.ExecuteNonQuery();
                        }

                        // Delete existing invoice details
                        string deleteDetailsSql = "DELETE FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                        using (SqlCommand command = new SqlCommand(deleteDetailsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                            command.ExecuteNonQuery();
                        }

                        // Insert new invoice details
                        foreach (var detail in invoice.InvoiceDetails)
                        {
                            string insertDetailSql = "INSERT INTO InvoiceDetails (InvoiceID, ProductID, Quantity, TotalPrice) VALUES (@InvoiceID, @ProductID, @Quantity, @TotalPrice)";
                            using (SqlCommand command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                                command.Parameters.AddWithValue("@ProductID", detail.ProductID);
                                command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                command.Parameters.AddWithValue("@TotalPrice", detail.Quantity * GetProductPrice(detail.ProductID));
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private decimal GetProductPrice(string productID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Price FROM Product WHERE ProductID = @ProductID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);
                    return (decimal)command.ExecuteScalar();
                }
            }
        }

        private SelectList GetCustomers()
        {
            List<CustomerInfo> customers = new List<CustomerInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT CustomerID, CustomerName FROM Customer";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new CustomerInfo
                            {
                                CustomerID = reader["CustomerID"].ToString(),
                                CustomerName = reader["CustomerName"].ToString()
                            });
                        }
                    }
                }
            }
            return new SelectList(customers, "CustomerID", "CustomerName");
        }

        private SelectList GetProducts()
        {
            List<ProductInfo> products = new List<ProductInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ProductID, ProductName FROM Product";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductInfo
                            {
                                ProductID = reader["ProductID"].ToString(),
                                ProductName = reader["ProductName"].ToString()
                            });
                        }
                    }
                }
            }
            return new SelectList(products, "ProductID", "ProductName");
        }
    }
}
