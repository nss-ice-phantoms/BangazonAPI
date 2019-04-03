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
    public class OrderTest
    {
        /************
         * GET Test
         ***********/
        [Fact]
        public async Task Test_Get_All_Orders()
        {

            using (var client = new APIClientProvider().Client)
            {
                /* ARRANGE */

                /* ACT */
                var response = await client.GetAsync("/api/orders");

                string responseBody = await response.Content.ReadAsStringAsync();
                var ordersList = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /* ASSERT */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(ordersList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var orderGetInitialResponse = await client.GetAsync("api/orders");
                string initialResponseBody = await orderGetInitialResponse.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Order>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, orderGetInitialResponse.StatusCode);
                var orderTypeObject = orderList[0];

                //BEGIN GET SPECIFIC TESTING
                var response = await client.GetAsync($"api/orders/{orderTypeObject.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                var orderReturned = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orderReturned.Id == orderTypeObject.Id);
            }
        }

        /*************
         * POST Test
         ************/
        [Fact]
        public async Task Test_Create_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*  ARRANGE */

                Order newOrder = new Order
                {
                    CustomerId = 1
                };

                var orderAsJSON = JsonConvert.SerializeObject(newOrder);


                /* ACT */

                var response = await client.PostAsync(
                    "/api/orders",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var returnedOrder = JsonConvert.DeserializeObject<Order>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1, returnedOrder.CustomerId);
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
                var orderGetInitialResponse = await client.GetAsync("api/orders");
                string initialResponseBody = await orderGetInitialResponse.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Order>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, orderGetInitialResponse.StatusCode);

                var orderObject = orderList[0];
                var defaultCustomerId = orderObject.CustomerId;

                /* PUT section */
                orderObject.CustomerId = 1;

                var modifiedOrderAsJson = JsonConvert.SerializeObject(orderObject);
                var response = await client.PutAsync($"api/orders/{orderObject.Id}",
                    new StringContent(modifiedOrderAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getOrder = await client.GetAsync($"api/orders/{orderObject.Id}");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);
                Assert.Equal(1, newOrder.CustomerId);

                newOrder.CustomerId = defaultCustomerId;
                var returnOrderToDefault = JsonConvert.SerializeObject(newOrder);

                var putOrderToDefault = await client.PutAsync($"api/orders/{newOrder.Id}",
                    new StringContent(returnOrderToDefault, Encoding.UTF8, "application/json"));

                string originalOrderObject = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            }
        }

        /*************
         * DELETE Test
         ************/
        [Fact]
        public async Task Test_Remove_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var orderGetInitialResponse = await client.GetAsync("api/orders");
                string initialResponseBody = await orderGetInitialResponse.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Order>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, orderGetInitialResponse.StatusCode);

                int removeLastObject = orderList.Count - 1;
                var orderObject = orderList[removeLastObject];

                var response = await client.DeleteAsync($"api/orders/{orderObject.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getOrder = await client.GetAsync($"api/orders/{orderObject.Id}");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);

                Assert.Equal(HttpStatusCode.NoContent, getOrder.StatusCode);
            }

        }


    }
}
