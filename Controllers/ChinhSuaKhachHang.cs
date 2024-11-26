using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ChinhSuaKhachHangController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditCustomer
        public ActionResult Index(string id)
        {
            ThongTinKhachHang customerInfo = new ThongTinKhachHang();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Customer WHERE CustomerID = @CustomerID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customerInfo.CustomerID = reader["CustomerID"].ToString();
                                customerInfo.CustomerName = reader["CustomerName"].ToString();
                                customerInfo.Phone = reader["Phone"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
                return View(customerInfo);
            }

            return View(customerInfo);
        }

        // POST: EditCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ThongTinKhachHang customerInfo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(customerInfo.CustomerID) || string.IsNullOrEmpty(customerInfo.CustomerName) || string.IsNullOrEmpty(customerInfo.Phone))
                {
                    errorMessage = "All the fields are required";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "UPDATE Customer SET CustomerName = @CustomerName, Phone = @Phone WHERE CustomerID = @CustomerID";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@CustomerID", customerInfo.CustomerID);
                            command.Parameters.AddWithValue("@CustomerName", customerInfo.CustomerName);
                            command.Parameters.AddWithValue("@Phone", customerInfo.Phone);
                            command.ExecuteNonQuery();
                        }
                    }
                    successMessage = "ChinhSuaKhachHangThanhCong";
                    ViewBag.SuccessMessage = successMessage;
                    return RedirectToAction("Index", "KhachHang");
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
