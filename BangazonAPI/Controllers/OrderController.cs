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

        // GET api/order
        [HttpGet(Name = "GetAllOrders")]
        public List<Order> GetAllOrders(string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //cmd.CommandText = "SELECT o.Id as OrderId, pt.[Name] as PaymentType";

                    //if (include == "products")
                    //{
                    //    cmd.CommandText += ", p.Id AS ProductId, p.Title AS ProductTitle, p.[Description]";
                    //}

                    //if (include == "customers")
                    //{
                    //    cmd.CommandText += ", c.Id AS CustomerId, c.FirstName, c.LastName";
                    //}

                    //cmd.CommandText += @" FROM [Order] o
                    //                    INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                    //                    INNER JOIN Customer c ON c.Id = o.CustomerId
                    //                    INNER JOIN OrderProduct op ON op.OrderId = o.Id
                    //                    INNER JOIN Product p ON p.Id = op.ProductId";

                    if (include == "products")
                    {
                        cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] AS PaymentType, p.Id AS ProductId, p.Title AS ProductTitle, p.[Description]
                                            FROM [Order] o
                                            INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                            INNER JOIN OrderProduct op ON op.OrderId = o.Id
                                            INNER JOIN Product p ON p.Id = op.ProductId";
                    }
                    if (include == "customers")
                    {
                        cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] AS PaymentType, c.Id AS CustomerId, c.FirstName, c.LastName
                                            FROM [Order] o
                                            INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                            INNER JOIN Customer c ON c.Id = o.CustomerId";
                    }
                    if (include != "customers" && include != "products")
                    {
                        cmd.CommandText = @"SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] as PaymentType
                                            FROM [Order] o
                                            INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId";
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Order> orders = new Dictionary<int, Order>();
                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                        if (!orders.ContainsKey(orderId))
                        {
                            Order newOrder = new Order
                            {
                                Id = orderId,
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                            };

                            orders.Add(orderId, newOrder);
                        }

                        if (include == "products")
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

                        if (include == "customers")
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
        // GET api/order/5
        [HttpGet("{id}", Name = "GetOrder")]
        public Order Get(int id, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "products")
                    {
                        cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] AS PaymentType, p.Id AS ProductId, p.Title AS ProductTitle, p.[Description]
                                            FROM [Order] o
                                            INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                            INNER JOIN OrderProduct op ON op.OrderId = o.Id
                                            INNER JOIN Product p ON p.Id = op.ProductId";
                    }
                    if (include == "customers" || include == "customer")
                    {
                        cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] AS PaymentType, c.Id AS CustomerId, c.FirstName, c.LastName
                                            FROM [Order] o
                                            INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                            INNER JOIN Customer c ON c.Id = o.CustomerId";
                    }
                    if (include == null)
                    {
                        cmd.CommandText = @"SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId, pt.[Name] as PaymentType
                                            FROM [Order] o
                                            INNER JOIN PaymentType pt ON pt.Id = o.PaymentTypeId";
                    }

                    cmd.CommandText += " WHERE o.Id = @id";
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

                        if (include == "products")
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

                        if (include == "customers" || include == "customer")
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
                    cmd.CommandText = @"INSERT INTO [Order] (CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@customerId, @paymentId)";
                    cmd.Parameters.Add(new SqlParameter("@customerId", newOrder.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@paymentId", newOrder.PaymentTypeId));

                    int newId = (int)cmd.ExecuteScalar(); 
                    newOrder.Id = newId;
                    return CreatedAtRoute("GetOrder", new { id = newId }, newOrder);
                }
            }
        }

        /**************
         * EDIT ORDER
        ***************/
        // PUT api/order/5
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
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
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
        // DELETE api/order/5
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
                        cmd.CommandText = @"DELETE FROM [Order] WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
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
