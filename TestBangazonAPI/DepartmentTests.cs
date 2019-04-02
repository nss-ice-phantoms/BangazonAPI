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

            using (var client = new APIClientProvider().Client) {

                var departmentGetInitialResponse = await client.GetAsync("api/departments");

                string initialResponseBody = await departmentGetInitialResponse.Content.ReadAsStringAsync();

                var departmentList = JsonConvert.DeserializeObject<List<Department>>(initialResponseBody);

                Assert.Equal(HttpStatusCode.OK, departmentGetInitialResponse.StatusCode);

                var departmentObject = departmentList[0];
                var defaultDepartmentName = departmentObject.Name;

                //BEGIN PUT TEST
                departmentObject.Name = "TestName";

                var modifiedDepartmentAsJson = JsonConvert.SerializeObject(departmentObject);

                var response = await client.PutAsync($"api/departments/{ departmentObject.Id}",
                    new StringContent(modifiedDepartmentAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getDepartment = await client.GetAsync($"api/departments/{ departmentObject.Id}");
                getDepartment.EnsureSuccessStatusCode();

                string getDepartmentBody = await getDepartment.Content.ReadAsStringAsync();
                Department newDepartment = JsonConvert.DeserializeObject<Department>(getDepartmentBody);

                Assert.Equal("TestName", newDepartment.Name);

                newDepartment.Name = defaultDepartmentName;
                var returnDepartmentToDefault = JsonConvert.SerializeObject(newDepartment);

                var putDepartmentToDefault = await client.PutAsync($"api/departments/{newDepartment.Id}",
                    new StringContent(returnDepartmentToDefault, Encoding.UTF8, "application/json"));

                string originalDepartmentObject = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); ;
            }

        }
    }
}
