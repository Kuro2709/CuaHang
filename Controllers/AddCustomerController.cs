using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class AddCustomerController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: AddCustomer
        public ActionResult Index()
        {
            return View(new CustomerInfo());
        }

        // POST: AddCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CustomerInfo customerInfo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(customerInfo.CustomerID) || string.IsNullOrEmpty(customerInfo.CustomerName) || string.IsNullOrEmpty(customerInfo.Phone))
                {
                    errorMessage = "Tất cả các mục đều bắt buộc, không được để trống";
                    ViewBag.ErrorMessage = errorMessage;
                    return View(customerInfo);
                }

                try
                {
                    string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
                    return View(new CustomerInfo());
                }

                catch (SqlException ex) when (ex.Number == 2627) // SQL error code for primary key violation
                {
                    errorMessage = "Mã khách hàng đã tồn tại, xin vui lòng nhập mã mới";
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





