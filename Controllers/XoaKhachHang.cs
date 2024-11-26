using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class XoaKhachHangController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteCustomer
        public ActionResult Index(string id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the customer exists in any invoice
                    string checkSql = "SELECT COUNT(*) FROM Invoice WHERE CustomerID = @CustomerID";
                    using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@CustomerID", id);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            errorMessage = "Cannot delete the customer as they exist in one or more invoices.";
                            ViewBag.ErrorMessage = errorMessage;
                            return RedirectToAction("Index", "KhachHang");
                        }
                    }

                    // Delete the customer
                    string deleteSql = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@CustomerID", id);
                        deleteCommand.ExecuteNonQuery();
                    }
                }
                successMessage = "Customer deleted successfully";
                ViewBag.SuccessMessage = successMessage;
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            return RedirectToAction("Index", "KhachHang");
        }
    }
}
