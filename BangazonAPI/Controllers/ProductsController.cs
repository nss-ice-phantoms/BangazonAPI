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

        //GET: api/products/5
        [HttpGet("{id}", Name = "GetProduct")]
        public Product get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.id, p.productTypeId, p.customerId, p.price, p.title, p.description, p.quantity " +
                                      "FROM Product p " +
                                      "WHERE p.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Product product = null;
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("productTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("customerId")),
                            Price = reader.GetInt32(reader.GetOrdinal("price")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
                        };
                    }
                    reader.Close();
                    return product;
                }
            }
        }

        //POST: api/products
        [HttpPost]
        public ActionResult Post([FromBody] Product newProduct)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO product (productTypeId, customerId, price, title, description, quantity) 
                                        OUTPUT INSERTED.Id 
                                        VALUES (@productTypeId, @customerId, @price, @title, @description, @quantity)";
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", newProduct.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@customerId", newProduct.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@price", newProduct.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", newProduct.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", newProduct.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", newProduct.Quantity));

                    int newId = (int) cmd.ExecuteScalar();
                    newProduct.Id = newId;
                    return CreatedAtRoute("GetProduct", new {id = newId}, newProduct);
                }
            }
        }

        //PUT: api/products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Product 
                                        SET productTypeId = @producttypeid,
                                            customerId = @customerid,
                                            price = @price,
                                            title = @title,
                                            description = @description,
                                            quantity = @quantity
                                        WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@producttypeid", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@customerid", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //DELETE api/products/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Product WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}