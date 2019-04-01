using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers {

    [Route("api/departments")]
    public class DepartmentController : Controller {

        private readonly IConfiguration configuration;

        public DepartmentController(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public SqlConnection Connection {
            get {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/departments
        [HttpGet]
        public IActionResult Get(string _include, string _filter, int _gt = 0) {

            string include = _include;
            string filter = _filter;
            int gt = _gt;

            List<Department> departments = new List<Department>();

            if (include != "employees") {

                using (SqlConnection conn = Connection) {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"SELECT id, Name, Budget
                                           FROM Department";

                        if (filter == "budget") {
                            cmd.CommandText += $@" WHERE Budget > {gt}";
                        }

                        SqlDataReader reader = cmd.ExecuteReader();


                        while (reader.Read()) {

                            Department department = new Department {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };

                            departments.Add(department);

                        }

                        reader.Close();
                    }
                    return Ok(departments);
                }
            }
            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Name, Budget
                                           FROM Department";

                    if (filter == "budget") {
                        cmd.CommandText += $@" WHERE Budget > {gt}";
                    }

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read()) {

                        Department department = new Department {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = new List<Employee>()
                        };

                        departments.Add(department);
                    }

                    reader.Close();

                    foreach (Department department in departments) {

                        cmd.CommandText = $@"SELECT e.id AS EmployeeId, e.FirstName AS FirstName, e.LastName AS LastName 
                                               FROM Employee e
                                          LEFT JOIN Department d on e.DepartmentId = d.Id
                                              WHERE d.Id = {department.Id}";

                        SqlDataReader reader2 = cmd.ExecuteReader();

                        while (reader2.Read()) {

                            Employee employee = new Employee {
                                Id = reader2.GetInt32(reader2.GetOrdinal("EmployeeId")),
                                FirstName = reader2.GetString(reader2.GetOrdinal("FirstName")),
                                LastName = reader2.GetString(reader2.GetOrdinal("LastName"))
                            };

                            department.Employees.Add(employee);
                        }

                        reader2.Close();
                    }
                }
                return Ok(departments);
            }
        }

        // GET api/departments/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public IActionResult Get(int id, string _include) {

            string include = _include;

            Department department = null;

            if (include != "employees") {

                using (SqlConnection conn = Connection) {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"SELECT id, Name, Budget
                                               FROM Department
                                              WHERE id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read()) {

                            department = new Department {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                        }
                        reader.Close();
                    }
                    return Ok(department);
                }
            } else {

                using (SqlConnection conn = Connection) {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"SELECT d.Id AS DepartmentId, d.Name AS DeptName, d.Budget AS Budget,
                                                    e.Id AS EmployeeId, e.FirstName AS FirstName, e.LastName AS LastName,
                                                    e.DepartmentId AS EmpDeptId
                                               FROM Department d
                                          LEFT JOIN Employee e ON d.Id = e.DepartmentId
                                              WHERE d.Id = @id";
                                              
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read()) {

                            if (department == null) {

                                department = new Department {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DeptName")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                    Employees = new List<Employee>()
                                };
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId"))) {

                                int employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));

                                if (!department.Employees.Any(e => e.Id == employeeId)) {

                                    Employee employee = new Employee {
                                        Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                        DepartmentId = reader.GetInt32(reader.GetOrdinal("EmpDeptId"))
                                    };

                                    department.Employees.Add(employee);
                                }
                            }
                        }
                        reader.Close();
                    }
                    return Ok(department);
                }
            }
        }

        // POST api/departments
        [HttpPost]
        public IActionResult Post([FromBody] Department department) {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"INSERT INTO Department (Name, Budget)
                                         OUTPUT INSERTED.Id
                                         VALUES (@name, @budget)
                                         SELECT MAX(Id) 
                                           FROM Department";

                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    int newId = (int)cmd.ExecuteScalar();
                    department.Id = newId;
                    return CreatedAtRoute("GetDepartment", new { id = newId }, department);
                }
            }
        }

        // PUT api/departments/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Department department) {

            try {

                using (SqlConnection conn = Connection) {

                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = $@"UPDATE Department
                                                SET Name = @name, Budget = @budget 
                                              WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0) {

                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            } catch (Exception) {

                if (!DepartmentExists(id)) {

                    return NotFound();
                }

                throw;
            }
        }

        private bool DepartmentExists(int id) {

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT id, Make, PurchaseDate, DecommissionDate, Manufacturer
                                           FROM Department
                                           WHERE id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
