using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class EditProductController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditProduct
        public ActionResult Index(string id)
        {
            ProductInfo productInfo = new ProductInfo();
            try
            {
                string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
        public ActionResult Index(ProductInfo productInfo)
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
                    string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
                    return RedirectToAction("Index", "Product");
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






