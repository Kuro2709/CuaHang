using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class EditCustomerController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: EditCustomer
        public ActionResult Index(string id)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            try
            {
                string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
        public ActionResult Index(CustomerInfo customerInfo)
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
                    string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True"; // Replace with your actual connection string
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
                    successMessage = "Customer updated successfully";
                    ViewBag.SuccessMessage = successMessage;
                    return RedirectToAction("Index", "Customer");
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






