using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ThemKhachHangController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: ThemKhachHang
        public ActionResult Index()
        {
            return View(new ThongTinKhachHang());
        }

        // POST: ThemKhachHang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ThongTinKhachHang customerInfo)
        {
            if (ModelState.IsValid)
            {
                // Trim spaces and convert CustomerID to uppercase
                customerInfo.CustomerID = customerInfo.CustomerID?.Trim().ToUpper();

                if (string.IsNullOrEmpty(customerInfo.CustomerID) || string.IsNullOrEmpty(customerInfo.CustomerName) || string.IsNullOrEmpty(customerInfo.Phone))
                {
                    errorMessage = "Tất cả các mục đều bắt buộc, không được để trống";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                if (customerInfo.CustomerID.Contains(" "))
                {
                    errorMessage = "Mã khách hàng không được chứa khoảng trắng";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(customerInfo.Phone, @"^\d+$"))
                {
                    errorMessage = "Số điện thoại chỉ được chứa số và không có khoảng trắng";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Customer (CustomerID, CustomerName, Phone) VALUES (@CustomerID, @CustomerName, @Phone)";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@CustomerID", customerInfo.CustomerID);
                            command.Parameters.AddWithValue("@CustomerName", customerInfo.CustomerName);
                            command.Parameters.AddWithValue("@Phone", customerInfo.Phone);
                            command.ExecuteNonQuery();
                        }
                    }
                    successMessage = "Khách hàng đã được thêm thành công";
                    ViewBag.SuccessMessage = successMessage;
                    ModelState.Clear();
                    return View(new ThongTinKhachHang());
                }
                catch (SqlException ex) when (ex.Number == 2627) // SQL error code for primary key violation
                {
                    errorMessage = "Mã khách hàng đã tồn tại, xin vui lòng nhập mã mới";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }
                catch (AggregateException ex)
                {
                    foreach (var innerEx in ex.InnerExceptions)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {innerEx.Message}");
                    }
                    errorMessage = "An error occurred while processing your request.";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }
            }

            return View(customerInfo);
        }
    }
}
