using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class DeleteCustomerController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteCustomer
        public ActionResult Index(string id)
        {
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"; // Replace with your actual connection string
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
                            return RedirectToAction("Index", "Customer");
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

            return RedirectToAction("Index", "Customer");
        }
    }
}
