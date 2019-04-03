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
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
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

        /*****************
         * GET ALL ORDERS
        ******************/

        // GET api/orders
        [HttpGet(Name = "GetAllOrders")]
        public List<Order> GetAllOrders(string _include, string _completed)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] as PaymentType";
                    
                    if (_include == "products")
                    {
                        cmd.CommandText += ", p.Id AS ProductId, p.Title AS ProductTitle, p.[Description]";
                    }

                    if (_include == "customers")
                    {
                        cmd.CommandText += ", c.Id AS CustomerId, c.FirstName, c.LastName";
                    }

                    cmd.CommandText += @" FROM [Order] o
                                        LEFT JOIN Customer c ON c.Id = o.CustomerId
                                        LEFT JOIN OrderProduct op ON op.OrderId = o.Id
                                        LEFT JOIN Product p ON p.Id = op.ProductId
                                        LEFT JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                        WHERE 1=1";

                    if (_completed == "false")
                    {
                        cmd.CommandText += " AND PaymentTypeId IS NULL";
                    }

                    if (_completed == "true")
                    {
                        cmd.CommandText += " AND PaymentTypeId IS NOT NULL";
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Order> orders = new Dictionary<int, Order>();
                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                        if (!orders.ContainsKey(orderId))
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                            {
                                Order newOrder = new Order
                                {
                                    Id = orderId,
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                                };

                                orders.Add(orderId, newOrder);
                            }
                            else
                            {
                                Order newOrder = new Order
                                {
                                    Id = orderId,
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                                };

                                orders.Add(orderId, newOrder);
                            }
                        }

                        if (_include == "products")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                Order currentOrder = orders[orderId];
                                currentOrder.ProductList.Add(
                                    new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        Title = reader.GetString(reader.GetOrdinal("ProductTitle")),
                                        Description = reader.GetString(reader.GetOrdinal("Description"))
                                    }
                                );
                            }
                        }

                        if (_include == "customers")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("CustomerId")))
                            {
                                Order currentOrder = orders[orderId];
                                currentOrder.Customer.Add(
                                    new Customer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                    }
                                );
                            }
                        }
                    }

                    reader.Close();
                    return orders.Values.ToList();

                }
            }
        }

        /********************
         * GET SINGLE ORDER
        ********************/
        // GET api/orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public Order Get(int id, string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] as PaymentType";

                    if (_include == "products")
                    {
                        cmd.CommandText += ", p.Id AS ProductId, p.Title AS ProductTitle, p.[Description]";
                    }

                    if (_include == "customers" || _include == "customer")
                    {
                        cmd.CommandText += ", c.Id AS CustomerId, c.FirstName, c.LastName";
                    }

                    cmd.CommandText += @" FROM [Order] o
                                        INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                        INNER JOIN Customer c ON c.Id = o.CustomerId
                                        INNER JOIN OrderProduct op ON op.OrderId = o.Id
                                        INNER JOIN Product p ON p.Id = op.ProductId
                                        WHERE o.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Order order = null;

                    while (reader.Read())
                    {
                        if (order == null)
                        {
                            order = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                            };
                        }

                        if (_include == "products")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                order.ProductList.Add(
                                    new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        Title = reader.GetString(reader.GetOrdinal("ProductTitle")),
                                        Description = reader.GetString(reader.GetOrdinal("Description"))
                                    }
                                );
                            }
                        }

                        if (_include == "customers" || _include == "customer")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("CustomerId")))
                            {
                                order.Customer.Add(
                                    new Customer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                    }
                                );
                            }
                        }
                    }

                    reader.Close();
                    return order;
                }
            }
        }

        /***************
         * CREATE ORDER
        ****************/

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order newOrder)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [Order] (CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@customerId)";
                    cmd.Parameters.Add(new SqlParameter("@customerId", newOrder.CustomerId));
                    //cmd.Parameters.Add(new SqlParameter("@paymentId", newOrder.PaymentTypeId));

                    int newId = (int)cmd.ExecuteScalar(); 
                    newOrder.Id = newId;
                    return CreatedAtRoute("GetOrder", new { id = newId }, newOrder);
                }
            }
        }

        /**************
         * EDIT ORDER
        ***************/
        // PUT api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE [Order]
                                            SET CustomerId = @customerId, PaymentTypeId = @paymentId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentId", order.PaymentTypeId));
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

        /***************
         * DELETE ORDER
        ****************/
        // DELETE api/orders/5
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
                        cmd.CommandText = @"DELETE FROM OrderProduct WHERE OrderId = @id
                                            DELETE FROM [Order] WHERE Id = @id";
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
                    cmd.CommandText = @"
                        SELECT Id, CustomerId, PaymentTypeId
                        FROM [Order]
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
