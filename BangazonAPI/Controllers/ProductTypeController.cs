// Author: Megan Cruzen

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
    public class ProductTypeController : ControllerBase
    {

        private readonly IConfiguration _config;

        public ProductTypeController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        /*************************
         * GET ALL PRODUCT TYPES
        **************************/
        // GET api/producttype
        [HttpGet(Name = "GetAllProductTypes")]
        public List<ProductType> GetAllProductTypes()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    
                    cmd.CommandText = @"SELECT Id, [Name] FROM ProductType";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, ProductType> productTypes = new Dictionary<int, ProductType>();
                    while (reader.Read())
                    {
                        int productTypeId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!productTypes.ContainsKey(productTypeId))
                        {
                            ProductType newProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            productTypes.Add(productTypeId, newProductType);
                        }
                    }

                    reader.Close();
                    return productTypes.Values.ToList();

                }
            }
        }

        /***************************
         * GET SINGLE PRODUCT TYPE
        ****************************/
        // GET api/producttype/5
        [HttpGet("{id}", Name = "GetProductType")]
        public ProductType Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, [Name] FROM ProductType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    ProductType productType = null;

                    if (reader.Read())
                    {
                        productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }

                    reader.Close();
                    return productType;
                }
            }
        }

 
        /**********************
         * CREATE PRODUCT TYPE
        **********************/
        // POST api/producttype
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductType newType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ProductType ([Name])
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.Add(new SqlParameter("@name", newType.Name));

                    int newId = (int)cmd.ExecuteScalar();
                    newType.Id = newId;
                    return CreatedAtRoute("GetOrder", new { id = newId }, newType);
                }
            }
        }


        /********************
         * EDIT PRODUCT TYPE
        ********************/
        // PUT api/producttype/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ProductType productType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ProductType
                                            SET [Name] = @name
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", productType.Name));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status200OK);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ObjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        /**********************
         * DELETE PRODUCT TYPE
        **********************/
        // DELETE api/producttype/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM ProductType WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status200OK);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ObjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ObjectExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, [Name]
                                        FROM ProductType
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
