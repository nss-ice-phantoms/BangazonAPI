using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ProductsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        //GET: api/products
        [HttpGet]
        public IActionResult Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.id, p.productTypeId, p.customerId, p.price, p.title, p.description, p.quantity " +
                                      "FROM Product p";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Product> products = new List<Product>();
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("productTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("customerId")),
                            Price = reader.GetInt32(reader.GetOrdinal("price")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
                        };
                        products.Add(product);
                    }

                    reader.Close();
                    if (products.Count == 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return Ok(products);
                    }
                }
            }
        }
    }
}