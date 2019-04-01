using System.Collections.Generic;
using Newtonsoft.Json;
using BangazonAPI.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System;

namespace TestBangazonAPI {

    public class ComputerTests {

        public int testComputerId = 4;

        [Fact]
        public async Task TestGetComputers() {

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */


                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.GetAsync("/api/computers");

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var computerList = JsonConvert.DeserializeObject<List<Computer>>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computerList.Count > 0);
            }
        }

        [Fact]
        public async Task TestCreateComputer() {

            using (var client = new APIClientProvider().Client) {

                /* ARRANGE */

                // Construct a new computer object to be sent to the API
                Computer computer = new Computer {
                    Make = "Test Computer",
                    Manufacturer = "Test",
                    PurchaseDate = DateTime.Now,
                    DecommissionDate = DateTime.Now.AddDays(365)
                };

                // Serialize the C# object into a JSON string
                var computerAsJSON = JsonConvert.SerializeObject(computer);

                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/computers",
                    new StringContent(computerAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Computer
                var newComputer = JsonConvert.DeserializeObject<Computer>(responseBody);

                testComputerId = newComputer.Id;

                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Test Computer", newComputer.Make);
                Assert.Equal("Test", newComputer.Manufacturer);
            }
        }

        [Fact]
        public async Task TestUpdateComputer() {

            // New computer name to change to and test
            string newComputerName = "Test Computer2";

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */

                var getComputerToUpdate = await client.GetAsync($"/api/computers/{testComputerId}");
                getComputerToUpdate.EnsureSuccessStatusCode();

                string getComputerToUpdateBody = await getComputerToUpdate.Content.ReadAsStringAsync();
                var computerToUpdate = JsonConvert.DeserializeObject<Computer>(getComputerToUpdateBody);

                int computerToUpdateId = computerToUpdate.Id;

                /*
                    PUT section
                */
                Computer modifiedComputer = new Computer {
                    Id = testComputerId,
                    Make = newComputerName,
                    Manufacturer = computerToUpdate.Manufacturer,
                    PurchaseDate = computerToUpdate.PurchaseDate,
                    DecommissionDate = computerToUpdate.DecommissionDate
                };

                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    $"/api/computers/{computerToUpdateId}",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getComputer = await client.GetAsync($"/api/computers/{computerToUpdateId}");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal("Test Computer2", newComputer.Make);
                Assert.Equal("Test", newComputer.Manufacturer);
            }
        }

        [Fact]
        public async Task TestDeleteComputer() {

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */

                /* ARRANGE */

                var getComputerToDelete = await client.GetAsync($"/api/computers/{testComputerId}");
                getComputerToDelete.EnsureSuccessStatusCode();

                string getComputerToDeleteBody = await getComputerToDelete.Content.ReadAsStringAsync();
                var computerToDelete = JsonConvert.DeserializeObject<Computer>(getComputerToDeleteBody);

                int computerToDeleteId = computerToDelete.Id;
                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.DeleteAsync($"/api/computers/{computerToDeleteId}");

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                /* ASSERT */

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getComputer = await client.GetAsync($"/api/computers/{computerToDeleteId}");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
            }

        }
    }
}