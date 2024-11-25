using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class AddProductController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: AddProduct
        public ActionResult Index()
        {
            return View(new ProductInfo());
        }

        // POST: AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProductInfo productInfo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(productInfo.ProductID) || string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
                {
                    errorMessage = "Tất cả các mục đều bắt buộc, không được để trống";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                try
                {
                    string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Product (ProductID, ProductName, Price) VALUES (@ProductID, @ProductName, @Price)";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@ProductID", productInfo.ProductID);
                            command.Parameters.AddWithValue("@ProductName", productInfo.ProductName);
                            command.Parameters.AddWithValue("@Price", productInfo.Price);
                            command.ExecuteNonQuery();
                        }
                    }
                    successMessage = "Sản phẩm đã được thêm thành công";
                    ViewBag.SuccessMessage = successMessage;
                    ModelState.Clear();
                    return View(new ProductInfo());
                }

                catch (SqlException ex) when (ex.Number == 2627) // SQL error code for primary key violation
                {
                    errorMessage = "Mã sản phẩm đã tồn tại, xin vui lòng nhập mã mới";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
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




