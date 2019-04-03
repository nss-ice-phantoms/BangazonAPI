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

                    reader.Close();
                    return employee;
                }
            }
        }


    }
}