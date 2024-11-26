using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Customer
        public ActionResult Index()
        {
            List<ThongTinKhachHang> listCustomers = new List<ThongTinKhachHang>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Customer";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThongTinKhachHang customerInfo = new ThongTinKhachHang
                                {
                                    CustomerID = reader["CustomerID"].ToString(),
                                    CustomerName = reader["CustomerName"].ToString(),
                                    Phone = reader["Phone"].ToString()
                                };
                                listCustomers.Add(customerInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return View(listCustomers);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(string id)
        {
            if (CustomerHasInvoices(id))
            {
                TempData["ErrorMessage"] = "Không thể xóa khách hàng. Khách hàng đang có trong một hoặc nhiều hóa đơn.";
                return RedirectToAction("Index");
            }

            // Proceed with deletion
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", id);
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

        private bool CustomerHasInvoices(string customerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Invoice WHERE CustomerID = @CustomerID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
