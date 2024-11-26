using System;
using Microsoft.Data.SqlClient;
using System.Web.Mvc;

namespace CuaHang.Controllers
{
    public class XoaHoaDonController : Controller
    {
        public string errorMessage = "";
        public string successMessage = "";

        // GET: DeleteInvoice
        public ActionResult Index(string id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceID", id);
                        command.ExecuteNonQuery();
                    }

                    sql = "DELETE FROM Invoice WHERE InvoiceID = @InvoiceID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceID", id);
                        command.ExecuteNonQuery();
                    }
                }
                successMessage = "Invoice deleted successfully";
                ViewBag.SuccessMessage = successMessage;
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
                ViewBag.ErrorMessage = errorMessage;
            }

            return RedirectToAction("Index", "HoaDon");
        }
    }
}










