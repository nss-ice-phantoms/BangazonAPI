using System.Collections.Generic;
using Newtonsoft.Json;
using BangazonAPI.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI {

    public class DepartmentTests {

        public int testDeptId = 6;

        [Fact]
        public async Task TestGetDepartments() {

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */


                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.GetAsync("/api/departments");

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var departmentList = JsonConvert.DeserializeObject<List<Department>>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departmentList.Count > 0);
            }
        }

        [Fact]
        public async Task TestCreateDepartment() {

            using (var client = new APIClientProvider().Client) {

                /* ARRANGE */

                // Construct a new department object to be sent to the API
                Department department = new Department {
                    Name = "Test Department",
                    Budget = 350000,
                };

                // Serialize the C# object into a JSON string
                var departmentAsJSON = JsonConvert.SerializeObject(department);

                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/departments",
                    new StringContent(departmentAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Department
                var newDepartment = JsonConvert.DeserializeObject<Department>(responseBody);

                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Test Department", newDepartment.Name);
                Assert.Equal(350000, newDepartment.Budget);
            }
        }

        [Fact]
        public async Task TestUpdateDepartment() {

            // New department name to change to and test
            string newDepartmentName = "Test Department2";

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */

                var getDepartmentToUpdate = await client.GetAsync($"/api/departments/{testDeptId}");
                getDepartmentToUpdate.EnsureSuccessStatusCode();

                string getDepartmentToUpdateBody = await getDepartmentToUpdate.Content.ReadAsStringAsync();
                var departmentToUpdate = JsonConvert.DeserializeObject<Department>(getDepartmentToUpdateBody);

                int departmentToUpdateId = departmentToUpdate.Id;

                /*
                    PUT section
                */
                Department modifiedDepartment = new Department {
                    Id = 6,
                    Name = newDepartmentName,
                    Budget = 350000,
                };

                var modifiedDepartmentAsJSON = JsonConvert.SerializeObject(modifiedDepartment);

                var response = await client.PutAsync(
                    $"/api/departments/{departmentToUpdateId}",
                    new StringContent(modifiedDepartmentAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getDepartment = await client.GetAsync($"/api/departments/{departmentToUpdateId}");
                getDepartment.EnsureSuccessStatusCode();

                string getDepartmentBody = await getDepartment.Content.ReadAsStringAsync();
                Department newDepartment = JsonConvert.DeserializeObject<Department>(getDepartmentBody);

                Assert.Equal(HttpStatusCode.OK, getDepartment.StatusCode);
                Assert.Equal(newDepartmentName, newDepartment.Name);
            }

        }
    }
}