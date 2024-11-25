using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class AddInvoiceController : Controller
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"; // Replace with your actual connection string

        // GET: AddInvoice
        public ActionResult Index()
        {
            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            return View(new InvoiceInfo { InvoiceDetails = new List<InvoiceDetailsInfo> { new InvoiceDetailsInfo() } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InvoiceInfo invoice)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate InvoiceID
                if (InvoiceExists(invoice.InvoiceID))
                {
                    ModelState.AddModelError("InvoiceID", "Invoice ID already exists.");
                }
                else
                {
                    // Calculate total price
                    invoice.RecalculateTotalPrice(GetProductPrice);

                    // Insert invoice and invoice details
                    InsertInvoice(invoice);
                    ViewBag.SuccessMessage = "Invoice added successfully.";
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            return View(invoice);
        }

        [HttpPost]
        public ActionResult AddInvoiceDetail()
        {
            ViewBag.Products = GetProducts();
            return PartialView("_InvoiceDetailRow", new InvoiceDetailsInfo());
        }

        private bool InvoiceExists(string invoiceID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Invoice WHERE InvoiceID = @InvoiceID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void InsertInvoice(InvoiceInfo invoice)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert invoice
                        string insertInvoiceSql = "INSERT INTO Invoice (InvoiceID, CustomerID, InvoiceDate, TotalPrice) VALUES (@InvoiceID, @CustomerID, @InvoiceDate, @TotalPrice)";
                        using (SqlCommand command = new SqlCommand(insertInvoiceSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                            command.Parameters.AddWithValue("@CustomerID", invoice.CustomerID);
                            command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                            command.Parameters.AddWithValue("@TotalPrice", invoice.TotalPrice);
                            command.ExecuteNonQuery();
                        }

                        // Insert invoice details
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

        [HttpGet]
        public JsonResult GetProductPriceByID(string productID)
        {
            decimal price = GetProductPrice(productID);
            return Json(price, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public JsonResult GetProductPriceById(string productID)
        {
            decimal price = GetProductPrice(productID);
            return Json(price, JsonRequestBehavior.AllowGet);
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
