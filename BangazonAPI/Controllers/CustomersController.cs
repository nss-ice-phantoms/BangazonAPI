using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public CustomersController(IConfiguration configuration)
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

        //GET: api/customers
        [HttpGet]
        public IActionResult Get(string include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT c.id, c.firstname, c.lastname FROM Customer c WHERE 1 = 1";

                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        cmd.CommandText += @"AND c.firstName LIKE @q or c.lastName LIKE @q";
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Customer> customers = new List<Customer>();
                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };
                        customers.Add(customer);
                    }

                    reader.Close();
                    if (customers.Count == 0)

                    {
                        return NoContent();
                    }
                    else
                    {
                        return Ok(customers);
                    }
                }
            }
        }

        //GET: api/customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public Customer Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.id, c.firstname, c.lastname FROM Customer c WHERE c.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Customer customer = null;
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                            LastName = reader.GetString(reader.GetOrdinal("lastname"))
                        };
                    }
                    reader.Close();
                    return customer;
                }
            }
        }

        // POST: api/customers
        [HttpPost]
        public ActionResult Post([FromBody] Customer newCustomer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        @"INSERT INTO Customer (firstName, lastName) OUTPUT INSERTED.Id VALUES (@firstname, @lastname)";

                    cmd.Parameters.Add(new SqlParameter("@firstname", newCustomer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", newCustomer.LastName));

                    int newId = (int) cmd.ExecuteScalar();
                    newCustomer.Id = newId;
                    return CreatedAtRoute("GetCustomer", new {id = newId}, newCustomer);
                }
            }
        }

        //PUT: api/Customers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Customer
                                        SET FirstName = @firstName,
                                            LastName = @lastName
                                        WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}