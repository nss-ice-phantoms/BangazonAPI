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
    public class PaymentTypesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public PaymentTypesController(IConfiguration configuration)
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

        //GET: api/PaymentTypes
        [HttpGet]
        public IActionResult Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.Id ,p.AcctNumber, p.Name, p.CustomerId" +
                                      " FROM PaymentType p";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<PaymentType> paymentTypes = new List<PaymentType>();
                    while (reader.Read())
                    {
                        PaymentType paymentType = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                        paymentTypes.Add(paymentType);

                    }
                    reader.Close();
                    if (paymentTypes.Count == 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return Ok(paymentTypes);
                    }

                }
            }
        }

        //GET: api/paymenttypes/5
        [HttpGet("{id}", Name = "GetPaymentType")]
        public PaymentType Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.Id ,p.AcctNumber, p.Name, p.CustomerId" +
                                      " FROM PaymentType p " +
                                      "WHERE p.id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    PaymentType paymentType = null;
                    if (reader.Read())
                    {
                        paymentType = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                    }
                    reader.Close();
                    return paymentType;
                }
            }
        }

        //POST: api/paymenttypes
        [HttpPost]
        public ActionResult Post([FromBody] PaymentType newPaymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO PaymentType (AcctNumber, Name, CustomerId) 
                                        OUTPUT INSERTED.Id 
                                        VALUES (@acctNumber, @name, @customerId)";
                    cmd.Parameters.Add(new SqlParameter("@acctNumber", newPaymentType.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@name", newPaymentType.Name));
                    cmd.Parameters.Add(new SqlParameter("@customerId", newPaymentType.CustomerId));

                    int newId = (int) cmd.ExecuteScalar();
                    newPaymentType.Id = newId;
                    return CreatedAtRoute("GetPaymentType", new {id = newId}, newPaymentType);
                }
            }
        }

        //PUT: api/paymentTypes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] PaymentType paymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE PaymentType 
                                            SET AcctNumber = @acctNumber,
                                                Name = @name,
                                                CustomerId = @customerId 
                                        WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@acctNumber", paymentType.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));
                    cmd.Parameters.Add(new SqlParameter("@customerId", paymentType.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
        //DELETE: api/paymentTypes/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM PaymentType WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}