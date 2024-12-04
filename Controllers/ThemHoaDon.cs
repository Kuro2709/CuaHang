using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Data.SqlClient;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ThemHoaDonController : Controller
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: ThemHoaDon
        public ActionResult Index()
        {
            ViewBag.Customers = GetCustomers();
            ViewBag.Products = GetProducts();
            return View(new ThongTinHoaDon { InvoiceDetails = new List<ThongTinChiTietHoaDon> { new ThongTinChiTietHoaDon() } });
        }

        // POST: ThemHoaDon
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
            invoice.InvoiceID = invoice.InvoiceID?.Trim().ToUpper().Replace(" ", "");

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
                    // Check for duplicate InvoiceID
                    if (InvoiceExists(invoice.InvoiceID))
                    {
                        ModelState.AddModelError("InvoiceID", "Mã hóa đơn đã tồn tại.");
                    }
                    else if (invoice.InvoiceDetails == null || invoice.InvoiceDetails.Count == 0)
                    {
                        ModelState.AddModelError("", "Bạn phải thêm ít nhất một dòng mặt hàng.");
                    }
                    else
                    {
                        // Calculate total price
                        invoice.RecalculateTotalPrice(GetProductPrice);

                        // Insert invoice and invoice details
                        InsertInvoice(invoice);
                        TempData["SuccessMessage"] = "Hóa đơn đã được thêm thành công.";
                        return RedirectToAction("Index");
                    }
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

        [HttpPost]
        public ActionResult AddInvoiceDetail()
        {
            ViewBag.Products = GetProducts();
            return PartialView("_InvoiceDetailRow", new ThongTinChiTietHoaDon());
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

        private void InsertInvoice(ThongTinHoaDon invoice)
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
    }
}
