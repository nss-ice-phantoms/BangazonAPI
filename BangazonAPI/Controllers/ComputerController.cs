using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers {

    [Route("api/computers")]
    public class ComputerController : Controller {

        private readonly IConfiguration configuration;

        public ComputerController(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public SqlConnection Connection {
            get {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/computers
        [HttpGet]
        public IActionResult Get() {

            using (SqlConnection conn = Connection) {

                List<Computer> computers = new List<Computer>();

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Make, PurchaseDate, DecommissionDate, Manufacturer
                                           FROM Computer";

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read()) {

                        Computer computer = new Computer {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))};

                        computers.Add(computer);

                    }

                    reader.Close();
                }
                return Ok(computers);
            }
        }

        // GET api/computers/5
        [HttpGet("{id}", Name = "GetComputer")]
        public IActionResult Get(int id) {

            using (SqlConnection conn = Connection) {

                Computer computer = new Computer();

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Make, PurchaseDate, DecommissionDate, Manufacturer
                                           FROM Computer
                                           WHERE id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {

                        computer = new Computer {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))};
                    }

                    reader.Close();
                }
                return Ok(computer);
            }
        }

        // POST api/computers
        [HttpPost]
        public IActionResult Post([FromBody] Computer computer) {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"INSERT INTO Computer (Make, PurchaseDate, DecommissionDate, Manufacturer)
                                         OUTPUT INSERTED.Id
                                         VALUES (@make, @purchaseDate, @decommissionDate, @manufacturer)
                                         SELECT MAX(Id) 
                                           FROM Computer";

                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decommissionDate", computer.DecommissionDate));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));

                    int newId = (int)cmd.ExecuteScalar();
                    computer.Id = newId;
                    return CreatedAtRoute("GetComputer", new { id = newId }, computer);
                }
            }
        }

        // PUT api/computers/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Computer computer) {

            try {

                using (SqlConnection conn = Connection) {

                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"UPDATE Computer
                                                SET Make = @make, PurchaseDate = @purchaseDate, 
                                                    DecommissionDate = @decommissionDate, 
                                                    Manufacturer = @manufacturer
                                              WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@decommissionDate", computer.DecommissionDate));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0) {

                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            } catch (Exception) {

                if (!ComputerExists(id)) {

                    return NotFound();
                }

                throw;
            }
        }

        // DELETE api/computers/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {

            try {

                using (SqlConnection conn = Connection) {

                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"DELETE FROM Computer 
                                                   WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0) {

                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            } catch (Exception) {

                if (!ComputerExists(id)) {

                    return NotFound();

                }

                throw;
            }
        }

        private bool ComputerExists(int id) {

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Make, PurchaseDate, DecommissionDate, Manufacturer
                                           FROM Computer
                                           WHERE id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
