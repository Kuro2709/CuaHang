using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ChinhSuaHoaDonController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: EditInvoice
        public ActionResult Index(string id)
        {
            ThongTinHoaDon invoice = new ThongTinHoaDon();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Fetch invoice header
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

                    // Fetch invoice details
                    invoice.InvoiceDetails = new List<ThongTinChiTietHoaDon>();
                    string detailsSql = "SELECT * FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                    using (SqlCommand command = new SqlCommand(detailsSql, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                invoice.InvoiceDetails.Add(new ThongTinChiTietHoaDon
                                {
                                    InvoiceID = reader["InvoiceID"].ToString(),
                                    ProductID = reader["ProductID"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                    Product = GetProductById(reader["ProductID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Exception: " + ex.Message;
                return View(invoice);
            }

            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            return View(invoice);
        }


        // POST: EditInvoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ThongTinHoaDon invoice, string[] ProductID, int[] Quantity)
        {
            // Log form data
            foreach (var key in Request.Form.AllKeys)
            {
                System.Diagnostics.Debug.WriteLine($"{key}: {Request.Form[key]}");
            }

            // Trim spaces and convert InvoiceID to uppercase
            invoice.InvoiceID = invoice.InvoiceID?.Trim().ToUpper();

            // Check for null values
            if (ProductID == null || Quantity == null)
            {
                ModelState.AddModelError("", "ProductID or Quantity is missing.");
                ViewBag.Customers = GetCustomers();
                ViewBag.Products = GetProducts();
                return View(invoice);
            }

            // Populate InvoiceDetails from form data
            invoice.InvoiceDetails = new List<ThongTinChiTietHoaDon>();
            for (int i = 0; i < ProductID.Length; i++)
            {
                var detail = new ThongTinChiTietHoaDon
                {
                    InvoiceID = invoice.InvoiceID,
                    ProductID = ProductID[i],
                    Quantity = Quantity[i],
                    TotalPrice = Quantity[i] * GetProductPrice(ProductID[i]),
                    Product = GetProductById(ProductID[i])
                };
                invoice.InvoiceDetails.Add(detail);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate total price
                    invoice.RecalculateTotalPrice(GetProductPrice);

                    // Update invoice and invoice details
                    UpdateInvoice(invoice);
                    TempData["SuccessMessage"] = "Hóa đơn đã được cập nhật thành công.";
                    return RedirectToAction("Index", new { id = invoice.InvoiceID });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            return View(invoice);
        }

        private void UpdateInvoice(ThongTinHoaDon invoice)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update invoice header (excluding InvoiceID)
                        string updateInvoiceSql = "UPDATE Invoice SET CustomerID = @CustomerID, InvoiceDate = @InvoiceDate, TotalPrice = @TotalPrice WHERE InvoiceID = @InvoiceID";
                        using (SqlCommand command = new SqlCommand(updateInvoiceSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                            command.Parameters.AddWithValue("@CustomerID", invoice.CustomerID);
                            command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                            command.Parameters.AddWithValue("@TotalPrice", invoice.TotalPrice);
                            command.ExecuteNonQuery();
                        }

                        // Delete existing details
                        string deleteDetailsSql = "DELETE FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                        using (SqlCommand command = new SqlCommand(deleteDetailsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                            command.ExecuteNonQuery();
                        }

                        // Insert updated details
                        foreach (var detail in invoice.InvoiceDetails)
                        {
                            string insertDetailSql = "INSERT INTO InvoiceDetails (InvoiceID, ProductID, Quantity, TotalPrice) VALUES (@InvoiceID, @ProductID, @Quantity, @TotalPrice)";
                            using (SqlCommand command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@InvoiceID", detail.InvoiceID);
                                command.Parameters.AddWithValue("@ProductID", detail.ProductID);
                                command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                command.Parameters.AddWithValue("@TotalPrice", detail.TotalPrice);
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

        private ThongTinSanPham GetProductById(string productId)
        {
            ThongTinSanPham product = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ProductID, ProductName, Price FROM Product WHERE ProductID = @ProductID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new ThongTinSanPham
                            {
                                ProductID = reader["ProductID"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"])
                            };
                        }
                    }
                }
            }
            return product;
        }

        private SelectList GetCustomers()
        {
            List<ThongTinKhachHang> customers = new List<ThongTinKhachHang>();
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
                            customers.Add(new ThongTinKhachHang
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
            List<ThongTinSanPham> products = new List<ThongTinSanPham>();
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
                            products.Add(new ThongTinSanPham
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

        [HttpPost]
        public ActionResult AddInvoiceDetail()
        {
            ViewBag.Products = GetProducts();
            return PartialView("_InvoiceDetailRow", new ThongTinChiTietHoaDon());
        }

        public ActionResult GetProductPriceByID(string productID)
        {
            decimal price = GetProductPrice(productID);
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInvoiceDetails(string id)
        {
            List<ThongTinChiTietHoaDon> invoiceDetails = new List<ThongTinChiTietHoaDon>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            invoiceDetails.Add(new ThongTinChiTietHoaDon
                            {
                                InvoiceID = reader["InvoiceID"].ToString(),
                                ProductID = reader["ProductID"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                Product = GetProductById(reader["ProductID"].ToString())
                            });
                        }
                    }
                }
            }
            return Json(invoiceDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateInvoiceDetail(ThongTinChiTietHoaDon detail)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE InvoiceDetails SET ProductID = @ProductID, Quantity = @Quantity, TotalPrice = @TotalPrice WHERE InvoiceDetailID = @InvoiceDetailID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceDetailID", detail.InvoiceDetailID);
                        command.Parameters.AddWithValue("@ProductID", detail.ProductID);
                        command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                        command.Parameters.AddWithValue("@TotalPrice", detail.TotalPrice);
                        command.ExecuteNonQuery();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values });
        }

        [HttpPost]
        public ActionResult DeleteInvoiceDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM InvoiceDetails WHERE InvoiceDetailID = @InvoiceDetailID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceDetailID", id);
                    command.ExecuteNonQuery();
                }
            }
            return Json(new { success = true });
        }
    }
}
