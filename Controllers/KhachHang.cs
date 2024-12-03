using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Data.SqlClient;
using CuaHang.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CuaHang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCustomers([DataSourceRequest] DataSourceRequest request)
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

            return Json(listCustomers.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCustomer([DataSourceRequest] DataSourceRequest request, ThongTinKhachHang customer)
        {
            if (customer != null)
            {
                if (CustomerHasInvoices(customer.CustomerID))
                {
                    ModelState.AddModelError("", "Không thể xóa khách hàng. Khách hàng đang có trong một hoặc nhiều hóa đơn.");
                }
                else
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sql = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
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

            return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
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




