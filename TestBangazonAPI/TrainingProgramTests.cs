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

        public int testTrainingProgramId = 4;

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

                testTrainingProgramId = newTrainingProgram.Id;

                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Test TrainingProgram", newTrainingProgram.Name);
            }
        }

        [Fact]
        public async Task TestUpdateTrainingProgram() {

            // New trainingProgram name to change to and test
            string newTrainingProgramName = "Test TrainingProgram2";

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */

                var getTrainingProgramToUpdate = await client.GetAsync($"/api/trainingprograms/{testTrainingProgramId}");
                getTrainingProgramToUpdate.EnsureSuccessStatusCode();

                string getTrainingProgramToUpdateBody = await getTrainingProgramToUpdate.Content.ReadAsStringAsync();
                var trainingProgramToUpdate = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramToUpdateBody);

                int trainingProgramToUpdateId = trainingProgramToUpdate.Id;

                /*
                    PUT section
                */
                TrainingProgram modifiedTrainingProgram = new TrainingProgram {
                    Id = testTrainingProgramId,
                    Name = newTrainingProgramName,
                    StartDate = trainingProgramToUpdate.StartDate,
                    EndDate = trainingProgramToUpdate.EndDate
                };

                var modifiedTrainingProgramAsJSON = JsonConvert.SerializeObject(modifiedTrainingProgram);

                var response = await client.PutAsync(
                    $"/api/trainingprograms/{trainingProgramToUpdateId}",
                    new StringContent(modifiedTrainingProgramAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getTrainingProgram = await client.GetAsync($"/api/trainingprograms/{trainingProgramToUpdateId}");
                getTrainingProgram.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getTrainingProgram.Content.ReadAsStringAsync();
                TrainingProgram newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getTrainingProgram.StatusCode);
                Assert.Equal("Test TrainingProgram2", newTrainingProgram.Name);
            }
        }

        [Fact]
        public async Task TestDeleteTrainingProgram() {

            using (var client = new APIClientProvider().Client) {
                /* ARRANGE */

                /* ARRANGE */

                var getTrainingProgramToDelete = await client.GetAsync($"/api/trainingprograms/{testTrainingProgramId}");
                getTrainingProgramToDelete.EnsureSuccessStatusCode();

                string getTrainingProgramToDeleteBody = await getTrainingProgramToDelete.Content.ReadAsStringAsync();
                var trainingProgramToDelete = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramToDeleteBody);

                int trainingProgramToDeleteId = trainingProgramToDelete.Id;
                /* ACT */

                // Use the client to send the request and store the response
                var response = await client.DeleteAsync($"/api/trainingprograms/{trainingProgramToDeleteId}");

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                /* ASSERT */

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getTrainingProgram = await client.GetAsync($"/api/trainingprograms/{trainingProgramToDeleteId}");
                getTrainingProgram.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getTrainingProgram.Content.ReadAsStringAsync();
                TrainingProgram newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getTrainingProgram.StatusCode);
            }

        }
    }
}