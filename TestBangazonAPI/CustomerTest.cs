using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Newtonsoft.Json;
using Xunit;

namespace TestBangazonAPI
{
    public class CustomerTest
    {
        [Fact]
        public async Task Test_GetAllCustomers()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/customers");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customerList = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customerList.Count > 0);
            }
        }


        
        [Fact]
        public async Task Test_GetSingleCustomer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var customerGetInitialResponse = await client.GetAsync("api/customers");
                string initialResponseBody = await customerGetInitialResponse.Content.ReadAsStringAsync();
                var customerList = JsonConvert.DeserializeObject<List<Customer>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK,customerGetInitialResponse.StatusCode);
                var customerObject = customerList[0];

                var response = await client.GetAsync($"api/customers/{customerObject.Id}");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customerReturned = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customerReturned.Id == customerObject.Id);
            }
        }

        [Fact]
        public async Task Test_Create_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Customer hernando = new Customer
                {
                    FirstName = "Hernando",
                    LastName = "Smith"
                };

                var hernAsJson = JsonConvert.SerializeObject(hernando);

                var response = await client.PostAsync("api/customers", 
                    new StringContent(hernAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                var newHerns = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Hernando", newHerns.FirstName);
                Assert.Equal("Smith", newHerns.LastName);
            }
        }

        [Fact]
        public async Task Test_Modify_Customer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var customerGetInitialResponse = await client.GetAsync("api/customers");

                string initialResponseBody = await customerGetInitialResponse.Content.ReadAsStringAsync();

                var customerList = JsonConvert.DeserializeObject<List<Customer>>(initialResponseBody);

                Assert.Equal(HttpStatusCode.OK, customerGetInitialResponse.StatusCode);

                var customerObject = customerList[0];

                var defaultCustomerLastName = customerObject.LastName;

                //PUT TEST BEGINS 
                customerObject.LastName = "newName";

                var modifiedCustomAsJson = JsonConvert.SerializeObject(customerObject);

                var response = await client.PutAsync($"api/customers/{customerObject.Id}",
                    new StringContent(modifiedCustomAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getCustomer = await client.GetAsync($"api/customers/{customerObject.Id}");
                getCustomer.EnsureSuccessStatusCode();

                string getCustomerBody = await getCustomer.Content.ReadAsStringAsync();
                Customer newCustomer = JsonConvert.DeserializeObject<Customer>(getCustomerBody);

                Assert.Equal("newName", newCustomer.LastName);
                
                newCustomer.LastName = defaultCustomerLastName;
                var returnCustomerToDefault = JsonConvert.SerializeObject(newCustomer);

                var putCustomerToDefault = await client.PutAsync($"api/customers/{customerObject.Id}",
                    new StringContent(returnCustomerToDefault, Encoding.UTF8, "application/json"));
                string originalCustomerObject = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
