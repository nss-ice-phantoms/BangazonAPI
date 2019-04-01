using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers {

    [Route("api/trainingprograms")]
    public class TrainingProgramController : Controller {

        private readonly IConfiguration configuration;

        public TrainingProgramController(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public SqlConnection Connection {
            get {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/trainingprograms
        [HttpGet]
        public IActionResult Get(string completed = "") {

            List<TrainingProgram> programs = new List<TrainingProgram>();

            DateTime today = DateTime.UtcNow;
            string filterDate = $"{today.Year}-{today.Month}-{today.Day}";

            string commandText = "";

            if (completed == "false") {

                commandText = $@"SELECT id, Name, StartDate, EndDate, MaxAttendees
                                   FROM TrainingProgram
                                   WHERE EndDate >= '{filterDate}'";
            } else {

                commandText = $@"SELECT id, Name, StartDate, EndDate, MaxAttendees
                                   FROM TrainingProgram";
            }

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = commandText;

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read()) {

                        TrainingProgram program = new TrainingProgram {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))};

                        programs.Add(program);

                    }

                    reader.Close();
                }
                return Ok(programs);
            }
        }

        // GET api/trainingprograms/5
        [HttpGet("{id}", Name = "GetProgram")]
        public IActionResult Get(int id) {

            TrainingProgram program = new TrainingProgram();

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Name, StartDate, EndDate, MaxAttendees
                                           FROM TrainingProgram
                                           WHERE id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {

                        program = new TrainingProgram {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))};
                    }

                    reader.Close();
                }
                return Ok(program);
            }

        }

        // POST api/trainingprograms
        [HttpPost]
        public IActionResult Post([FromBody] TrainingProgram program) {

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                         OUTPUT INSERTED.Id
                                         VALUES (@name, @startDate, @endDate, @maxAttendees)
                                         SELECT MAX(Id) 
                                           FROM TrainingProgram";

                    cmd.Parameters.Add(new SqlParameter("@name", program.Name));
                    cmd.Parameters.Add(new SqlParameter("@startDate", program.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", program.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@maxAttendees", program.MaxAttendees));

                    int newId = (int)cmd.ExecuteScalar();
                    program.Id = newId;
                    return CreatedAtRoute("GetProgram", new { id = newId }, program);
                }
            }
        }

        // PUT api/trainingprograms/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TrainingProgram program) {

            try {

                using (SqlConnection conn = Connection) {

                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"UPDATE TrainingProgram
                                                SET Name = @Name, StartDate = @StartDate, EndDate = @EndDate, 
                                                    MaxAttendees = @MaxAttendees
                                              WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@Name", program.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", program.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", program.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", program.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0) {

                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            } catch (Exception) {

                if (!ProgramExists(id)) {

                    return NotFound();
                }

                throw;
            }
        }

        // DELETE api/trainingprograms/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {

            try {

                using (SqlConnection conn = Connection) {

                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"DELETE FROM TrainingProgram 
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

                if (!ProgramExists(id)) {

                    return NotFound();

                }

                throw;
            }
        }

        private bool ProgramExists(int id) {

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Name, StartDate, EndDate, MaxAttendees
                                           FROM TrainingProgram
                                           WHERE id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}
