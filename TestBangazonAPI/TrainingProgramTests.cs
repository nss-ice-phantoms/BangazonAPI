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

    public class TrainingProgramTests {

        [Fact]
        public async Task TestGetTrainingPrograms() {

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */


                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.GetAsync("/api/trainingprograms");

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var trainingProgramList = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingProgramList.Count > 0);
            }
        }

        [Fact]
        public async Task TestCreateTrainingProgram() {

            using (var client = new APIClientProvider().Client) {

                /* ARRANGE */

                // Construct a new trainingProgram object to be sent to the API
                TrainingProgram trainingProgram = new TrainingProgram {
                    Name = "Test TrainingProgram",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(365)
                };

                // Serialize the C# object into a JSON string
                var trainingProgramAsJSON = JsonConvert.SerializeObject(trainingProgram);

                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/trainingprograms",
                    new StringContent(trainingProgramAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of TrainingProgram
                var newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Test TrainingProgram", newTrainingProgram.Name);
            }
        }

        [Fact]
        public async Task TestUpdateTrainingProgram() {

            using (var client = new APIClientProvider().Client) {

                var trainingProgramGetInitialResponse = await client.GetAsync("api/trainingprograms");

                string initialResponseBody = await trainingProgramGetInitialResponse.Content.ReadAsStringAsync();

                var trainingProgramList = JsonConvert.DeserializeObject<List<TrainingProgram>>(initialResponseBody);

                Assert.Equal(HttpStatusCode.OK, trainingProgramGetInitialResponse.StatusCode);

                var trainingProgramObject = trainingProgramList[0];
                var defaultTrainingProgramName = trainingProgramObject.Name;

                //BEGIN PUT TEST
                trainingProgramObject.Name = "TestName";

                var modifiedTrainingProgramAsJson = JsonConvert.SerializeObject(trainingProgramObject);

                var response = await client.PutAsync($"api/trainingprograms/{trainingProgramObject.Id}",
                    new StringContent(modifiedTrainingProgramAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getTrainingProgram = await client.GetAsync($"api/trainingprograms/{trainingProgramObject.Id}");
                getTrainingProgram.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getTrainingProgram.Content.ReadAsStringAsync();
                TrainingProgram newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramBody);

                Assert.Equal("TestName", newTrainingProgram.Name);

                newTrainingProgram.Name = defaultTrainingProgramName;
                var returnTrainingProgramToDefault = JsonConvert.SerializeObject(newTrainingProgram);

                var putTrainingProgramToDefault = await client.PutAsync($"api/trainingprograms/{newTrainingProgram.Id}",
                    new StringContent(returnTrainingProgramToDefault, Encoding.UTF8, "application/json"));

                string originalTrainingProgramObject = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); ;
            }
        }

        [Fact]
        public async Task TestDeleteTrainingProgram() {

            using (var client = new APIClientProvider().Client) {

                var trainingProgramGetInitialResponse = await client.GetAsync("api/trainingprograms");

                string initialResponseBody = await trainingProgramGetInitialResponse.Content.ReadAsStringAsync();

                var trainingProgramList = JsonConvert.DeserializeObject<List<TrainingProgram>>(initialResponseBody);

                Assert.Equal(HttpStatusCode.OK, trainingProgramGetInitialResponse.StatusCode);

                int removeLastObject = trainingProgramList.Count - 1;
                var trainingProgramObject = trainingProgramList[removeLastObject];

                var response = await client.DeleteAsync($"api/trainingprograms/{ trainingProgramObject.Id}");

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getTrainingProgram = await client.GetAsync($"api/trainingprograms/{ trainingProgramObject.Id}");
                getTrainingProgram.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getTrainingProgram.Content.ReadAsStringAsync();

                TrainingProgram newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramBody);

                Assert.Equal(HttpStatusCode.NoContent, getTrainingProgram.StatusCode);
            }

        }
    }
}
