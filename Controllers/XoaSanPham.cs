using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class XoaSanPhamController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteProduct
        public ActionResult Index(string id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; // Replace with your actual connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the product exists in any invoice
                    string checkSql = "SELECT COUNT(*) FROM InvoiceDetails WHERE ProductID = @ProductID";
                    using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ProductID", id);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            errorMessage = "Cannot delete the product as it exists in one or more invoices.";
                            ViewBag.ErrorMessage = errorMessage;
                            return RedirectToAction("Index", "SanPham");
                        }
                    }

                    // Delete the product
                    string deleteSql = "DELETE FROM Product WHERE ProductID = @ProductID";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@ProductID", id);
                        deleteCommand.ExecuteNonQuery();
                    }
                }
                successMessage = "Product deleted successfully";
                ViewBag.SuccessMessage = successMessage;
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            return RedirectToAction("Index", "SanPham");
        }
    }
}
