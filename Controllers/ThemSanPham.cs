using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ThemSanPhamController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: ThemSanPham
        public ActionResult Index()
        {
            return View(new ThongTinSanPham());
        }

        // POST: ThemSanPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ThongTinSanPham productInfo)
        {
            if (ModelState.IsValid)
            {
                // Trim spaces and convert ProductID to uppercase
                productInfo.ProductID = productInfo.ProductID?.Trim().ToUpper();

                if (string.IsNullOrEmpty(productInfo.ProductID) || string.IsNullOrEmpty(productInfo.ProductName) || productInfo.Price <= 0)
                {
                    errorMessage = "Tất cả các mục đều bắt buộc, không được để trống";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                if (productInfo.ProductID.Contains(" "))
                {
                    errorMessage = "Mã sản phẩm không được chứa khoảng trắng";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }

                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
                    return View(new ThongTinSanPham());
                }
                catch (SqlException ex) when (ex.Number == 2627) // SQL error code for primary key violation
                {
                    errorMessage = "Mã sản phẩm đã tồn tại, xin vui lòng nhập mã mới";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(productInfo);
                }
                catch (AggregateException ex)
                {
                    foreach (var innerEx in ex.InnerExceptions)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {innerEx.Message}");
                    }
                    errorMessage = "An error occurred while processing your request.";
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
