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
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public OrdersController(IConfiguration configuration)
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

        [HttpGet]
        public IActionResult Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, customerId, paymentTypeId FROM [order];";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Order> orders = new List<Order>();
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("customerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("paymentTypeId"))
                        };
                        orders.Add(order);
                    }
                    reader.Close();
                    if (orders.Count == 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return Ok(orders);
                    }
                }
            }
        }
    }
}