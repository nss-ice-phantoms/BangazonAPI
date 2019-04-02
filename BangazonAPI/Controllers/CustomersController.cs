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
        public IActionResult Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (string.IsNullOrWhiteSpace(_include))
                    {
                        cmd.CommandText = "SELECT c.id, c.firstname, c.lastname FROM Customer c";

                        //CONDITIONAL STATEMENT FOR q-search-----
                        if (!string.IsNullOrWhiteSpace(q))
                        {
                            cmd.CommandText += @" WHERE 1 = 1 AND c.firstName LIKE @q or c.lastName LIKE @q";
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

                        //ELSE
                    }
                    else
                    {
                        if(_include == "products") { 
                        cmd.CommandText =
                            "SELECT c.id AS CustomerId, c.firstname, c.lastname, p.id AS ProductId, p.ProductTypeId, p.CustomerId AS ProductsCustomerId, p.Price, p.Title, p.Description, p.Quantity, pt.id AS PaymentTypeId, pt.AcctNumber, pt.Name, pt.CustomerId AS PaymentTypeCustomerId " +
                            "FROM Customer c " +
                            "LEFT JOIN Product p " +
                            "ON c.id = p.CustomerId " +
                            "LEFT JOIN PaymentType pt " +
                            "ON c.id = pt.CustomerId; ";

                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
                        Dictionary<int, Product> productSort = new Dictionary<int, Product>();
                        while (reader.Read())
                        {
                            int customerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                            if (!customers.ContainsKey(customerId))
                            {
                                Customer newCustomer = new Customer
                                {
                                    Id = customerId,
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };
                                customers.Add(customerId, newCustomer);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                                if (!productSort.ContainsKey(productId))
                                {
                                    Product newProduct = new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("ProductsCustomerId")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                    };
                                    productSort.Add(productId,newProduct);

                                    Customer currentCustomer = customers[customerId];
                                    currentCustomer.ProductList.Add(
                                        new Product
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                            CustomerId = reader.GetInt32(reader.GetOrdinal("ProductsCustomerId")),
                                            Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                            Title = reader.GetString(reader.GetOrdinal("Title")),
                                            Description = reader.GetString(reader.GetOrdinal("Description")),
                                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                        }
                                        );
                                }

                            }
                        }

                        reader.Close();
                        if (customers.Count == 0)

                        {
                            return NoContent();
                        }
                        else
                        {
                            return Ok(customers.Values.ToList());
                        }
                    } else if (_include == "payments")
                        {
                            cmd.CommandText =
                                "SELECT c.id AS CustomerId, c.firstname, c.lastname, p.id AS ProductId, p.ProductTypeId, p.CustomerId AS ProductsCustomerId, p.Price, p.Title, p.Description, p.Quantity, pt.id AS PaymentTypeId, pt.AcctNumber, pt.Name, pt.CustomerId AS PaymentTypeCustomerId " +
                                "FROM Customer c " +
                                "LEFT JOIN Product p " +
                                "ON c.id = p.CustomerId " +
                                "LEFT JOIN PaymentType pt " +
                                "ON c.id = pt.CustomerId; ";

                            SqlDataReader reader = cmd.ExecuteReader();

                            Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
                            Dictionary<int, PaymentType> paymentTypesSort = new Dictionary<int, PaymentType>();
                            while (reader.Read())
                        {
                            int customerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                            if (!customers.ContainsKey(customerId))
                            {
                                Customer newCustomer = new Customer
                                {
                                    Id = customerId,
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };
                                customers.Add(customerId, newCustomer);
                            }

                                if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                {
                                    int paymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"));
                                    if (!paymentTypesSort.ContainsKey(paymentTypeId))
                                    {
                                        PaymentType newPaymentType = new PaymentType
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                            Name = reader.GetString(reader.GetOrdinal("Name")),
                                            CustomerId = reader.GetInt32(reader.GetOrdinal("PaymentTypeCustomerId"))
                                        };
                                        paymentTypesSort.Add(paymentTypeId, newPaymentType);

                                        Customer currentCustomer = customers[customerId];
                                        currentCustomer.PaymentTypeList.Add(
                                            new PaymentType
                                            {
                                                Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                                CustomerId = reader.GetInt32(reader.GetOrdinal("PaymentTypeCustomerId"))
                                            }
                                            );
                                    }

                                }
                            }

                        reader.Close();
                        if (customers.Count == 0)

                        {
                            return NoContent();
                        }
                        else
                        {
                            return Ok(customers.Values.ToList());
                        }
                        }
                        else
                        {
                            return NoContent();
                        }
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