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

        [Fact]
        public async Task TestGetComputers() {

            using (var client = new APIClientProvider().Client) {
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
                var response = await client.PostAsync("/api/computers",
                    new StringContent(computerAsJSON, Encoding.UTF8, "application/json"));

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Computer
                var newComputer = JsonConvert.DeserializeObject<Computer>(responseBody);

                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Test Computer", newComputer.Make);
                Assert.Equal("Test", newComputer.Manufacturer);
            }
        }

        [Fact]
        public async Task TestUpdateComputer() {
       
            using (var client = new APIClientProvider().Client) {
            
                var computerGetInitialResponse = await client.GetAsync("api/computers");
                 string initialResponseBody = await computerGetInitialResponse.Content.ReadAsStringAsync();
                 var computerList = JsonConvert.DeserializeObject<List<Computer>>(initialResponseBody);
                 Assert.Equal(HttpStatusCode.OK, computerGetInitialResponse.StatusCode);                  var computerObject = computerList[0];                 var defaultComputerMake = computerObject.Make;                  //BEGIN PUT TEST                 computerObject.Make = "TestName";
                 var modifiedComputerAsJson = JsonConvert.SerializeObject(computerObject);
                 var response = await client.PutAsync($"api/computers/{ computerObject.Id}",                     new StringContent(modifiedComputerAsJson, Encoding.UTF8, "application/json"));                  string responseBody = await response.Content.ReadAsStringAsync();                  Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);                  var getComputer = await client.GetAsync($"api/computers/{ computerObject.Id}");                 getComputer.EnsureSuccessStatusCode();                  string getComputerBody = await getComputer.Content.ReadAsStringAsync();                 Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);                  Assert.Equal("TestName", newComputer.Make);
                 newComputer.Make = defaultComputerMake;                 var returnComputerToDefault = JsonConvert.SerializeObject(newComputer);                  var putComputerToDefault = await client.PutAsync($"api/computers/{newComputer.Id}",                     new StringContent(returnComputerToDefault, Encoding.UTF8, "application/json"));
                 string originalComputerObject = await response.Content.ReadAsStringAsync();
                 Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);;
            }
        }

        [Fact]
        public async Task TestDeleteComputer() {

            using (var client = new APIClientProvider().Client) {
           
                var computerGetInitialResponse = await client.GetAsync("api/computers");
                 string initialResponseBody = await computerGetInitialResponse.Content.ReadAsStringAsync();
                 var computerList = JsonConvert.DeserializeObject<List<Computer>>(initialResponseBody);
                 Assert.Equal(HttpStatusCode.OK, computerGetInitialResponse.StatusCode);
                 int removeLastObject = computerList.Count - 1;                 var computerObject = computerList[removeLastObject];                  var response = await client.DeleteAsync($"api/computers/{ computerObject.Id}");                  string responseBody = await response.Content.ReadAsStringAsync();                  Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);                  var getComputer = await client.GetAsync($"api/computers/{ computerObject.Id}");                 getComputer.EnsureSuccessStatusCode();                  string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                 Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);                  Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
            }

        }
    }
}
