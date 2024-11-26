using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ChinhSuaSanPhamController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditProduct
        public ActionResult Index(string id)
        {
            ThongTinSanPham productInfo = new ThongTinSanPham();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Product WHERE ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                productInfo.ProductID = reader["ProductID"].ToString();
                                productInfo.ProductName = reader["ProductName"].ToString();
                                productInfo.Price = Convert.ToDecimal(reader["Price"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
                return View(productInfo);
            }

            return View(productInfo);
        }

        // POST: EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ThongTinSanPham productInfo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(productInfo.ProductID) || string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
                {
                    errorMessage = "All the fields are required";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "UPDATE Product SET ProductName = @ProductName, Price = @Price WHERE ProductID = @ProductID";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@ProductID", productInfo.ProductID);
                            command.Parameters.AddWithValue("@ProductName", productInfo.ProductName);
                            command.Parameters.AddWithValue("@Price", productInfo.Price);
                            command.ExecuteNonQuery();
                        }
                    }
                    successMessage = "Product updated successfully";
                    ViewBag.SuccessMessage = successMessage;
                    return RedirectToAction("Index", "SanPham");
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }
            }

            return View(productInfo);
        }
    }
}
