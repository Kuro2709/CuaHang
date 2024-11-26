using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Product
        public ActionResult Index()
        {
            List<ThongTinSanPham> listProducts = new List<ThongTinSanPham>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Product";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThongTinSanPham product = new ThongTinSanPham
                                {
                                    ProductID = reader["ProductID"].ToString(),
                                    ProductName = reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"])
                                };
                                listProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return View(listProducts);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(string id)
        {
            if (ProductHasInvoices(id))
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm. Sản phẩm đang có trong một hoặc nhiều hóa đơn.";
                return RedirectToAction("Index");
            }

            // Proceed with deletion
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Product WHERE ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        private bool ProductHasInvoices(string productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM InvoiceDetails WHERE ProductID = @ProductID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
