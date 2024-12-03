using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Data.SqlClient;
using CuaHang.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CuaHang.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProducts([DataSourceRequest] DataSourceRequest request)
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

            return Json(listProducts.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteProduct([DataSourceRequest] DataSourceRequest request, ThongTinSanPham product)
        {
            if (product != null)
            {
                if (ProductHasInvoices(product.ProductID))
                {
                    ModelState.AddModelError("", "Không thể xóa sản phẩm. Sản phẩm đang có trong một hoặc nhiều hóa đơn.");
                }
                else
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sql = "DELETE FROM Product WHERE ProductID = @ProductID";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@ProductID", product.ProductID);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
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

