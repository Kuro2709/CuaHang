using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            List<ProductInfo> listProducts = new List<ProductInfo>();

            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"; // Replace with your actual connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Product";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductInfo product = new ProductInfo();
                                product.ProductID = reader["ProductID"].ToString();
                                product.ProductName = reader["ProductName"].ToString();
                                product.Price = Convert.ToDecimal(reader["Price"]);
                                listProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return View(listProducts);
        }

        // Other actions (Create, Edit, Delete, etc.) go here
    }
}
