// Author: Megan Cruzen

using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class EmployeeTest
    {
        /************
         * GET Test
         ***********/
        [Fact]
        public async Task Test_Get_All_Employees()
        {

            using (var client = new APIClientProvider().Client)
            {
                /* ARRANGE */

                /* ACT */
                var response = await client.GetAsync("/api/employees");

                string responseBody = await response.Content.ReadAsStringAsync();
                var ordersList = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                /* ASSERT */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(ordersList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                var employeeGetInitialResponse = await client.GetAsync("api/employees");
                string initialResponseBody = await employeeGetInitialResponse.Content.ReadAsStringAsync();
                var employeeList = JsonConvert.DeserializeObject<List<Employee>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, employeeGetInitialResponse.StatusCode);
                var employeeObject = employeeList[0];

                //BEGIN GET SPECIFIC TESTING
                var response = await client.GetAsync($"api/employees/{employeeObject.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                var employeeReturned = JsonConvert.DeserializeObject<Employee>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employeeReturned.Id == employeeObject.Id);
            }
        }

        /*************
         * POST Test
         ************/
        [Fact]
        public async Task Test_Create_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*  ARRANGE */

                Employee newEmployee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    DepartmentId = 1
                };

                var employeeAsJSON = JsonConvert.SerializeObject(newEmployee);


                /* ACT */

                var response = await client.PostAsync(
                    "/api/employees",
                    new StringContent(employeeAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var returnedEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("John", returnedEmployee.FirstName);
                Assert.Equal("Doe", returnedEmployee.LastName);
                Assert.Equal(1, returnedEmployee.DepartmentId);
            }
        }

        /*************
         * PUT Test
         ************/
        [Fact]
        public async Task Test_Modify_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var employeeGetInitialResponse = await client.GetAsync("api/employees");
                string initialResponseBody = await employeeGetInitialResponse.Content.ReadAsStringAsync();
                var employeeList = JsonConvert.DeserializeObject<List<Employee>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, employeeGetInitialResponse.StatusCode);

                var employeeObject = employeeList[0];
                var defaultEmployeeFirstName = employeeObject.FirstName;

                /* PUT section */
                employeeObject.FirstName = "Tester";

                var modifiedEmployeeAsJson = JsonConvert.SerializeObject(employeeObject);
                var response = await client.PutAsync($"api/employees/{employeeObject.Id}",
                    new StringContent(modifiedEmployeeAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getEmployee = await client.GetAsync($"api/employees/{employeeObject.Id}");
                getEmployee.EnsureSuccessStatusCode();

                string getEmployeeBody = await getEmployee.Content.ReadAsStringAsync();
                Employee newEmployee = JsonConvert.DeserializeObject<Employee>(getEmployeeBody);
                Assert.Equal("Tester", newEmployee.FirstName);

                newEmployee.FirstName = defaultEmployeeFirstName;
                var returnEmployeeToDefault = JsonConvert.SerializeObject(newEmployee);

                var putEmployeeToDefault = await client.PutAsync($"api/employees/{newEmployee.Id}",
                    new StringContent(returnEmployeeToDefault, Encoding.UTF8, "application/json"));

                string originalEmployeeObject = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            }
        }
    }
}
