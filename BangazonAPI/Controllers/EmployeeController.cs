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
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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

        /*********************
         * GET ALL EMPLOYEES
        *********************/
        // GET api/employees
        [HttpGet(Name = "GetAllEmployees")]
        public List<Employee> GetAllEmployees()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId, d.[Name] as DeptName, 
                                            c.Id AS ComputerId, c.Make, c.Manufacturer, ce.AssignDate, ce.UnassignDate
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
                                        LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                        WHERE UnassignDate IS NULL";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
                    while (reader.Read())
                    {
                        int employeeId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!employees.ContainsKey(employeeId))
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                            {
                                Employee newEmployee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Department = new Department
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                        Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                    },
                                    Computer = new Computer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                    }
                                };
                                employees.Add(employeeId, newEmployee);
                            }
                            else
                            {
                                Employee newEmployee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Department = new Department
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                        Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                    }
                                };
                                employees.Add(employeeId, newEmployee);
                            }
                        }

                    }

                    reader.Close();
                    return employees.Values.ToList();

                }
            }
        }

        /***********************
         * GET SINGLE EMPLOYEE
        ************************/
        // GET api/employees/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public Employee Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId, d.[Name] as DeptName, 
                                            c.Id AS ComputerId, c.Make, c.Manufacturer, ce.AssignDate, ce.UnassignDate
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
                                        LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                        WHERE e.Id = @id AND UnassignDate IS NULL";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                },
                                Computer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                }
                            };
                        }
                        else
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                }
                            };
                        }
                    }

                    reader.Close();
                    return employee;
                }
            }
        }

        /*******************
         * CREATE EMPLOYEE
        *******************/
        // POST api/employees
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee newEmployee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, IsSuperVisor, DepartmentId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@firstname, @lastname, @supervisor, @deptId)";
                    cmd.Parameters.Add(new SqlParameter("@firstname", newEmployee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", newEmployee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@supervisor", newEmployee.IsSuperVisor));
                    cmd.Parameters.Add(new SqlParameter("@deptId", newEmployee.DepartmentId));

                    int newId = (int)cmd.ExecuteScalar();
                    newEmployee.Id = newId;
                    return CreatedAtRoute("GetEmployee", new { id = newId }, newEmployee);
                }
            }
        }

        /*****************
         * EDIT EMPLOYEE
        *****************/
        // PUT api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET FirstName = @firstname, LastName = @lastname, IsSuperVisor = @supervisor, DepartmentId = @deptid
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstname", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@supervisor", employee.IsSuperVisor));
                        cmd.Parameters.Add(new SqlParameter("@deptId", employee.DepartmentId));
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
                    cmd.CommandText = @"SELECT Id, FirstName, LastName
                                        FROM Employee
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }


    }
}