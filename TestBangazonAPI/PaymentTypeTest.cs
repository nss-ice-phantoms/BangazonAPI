using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Newtonsoft.Json;
using Xunit;

namespace TestBangazonAPI
{
    public class PaymentTypeTest
    {
        [Fact]
        public async Task Test_GetAllPaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/paymenttypes");

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypeList = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_GetSinglePaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                var paymentTypeGetInitialResponse = await client.GetAsync("api/paymentTypes");
                string initialResponseBody = await paymentTypeGetInitialResponse.Content.ReadAsStringAsync();
                var paymentTypeList = JsonConvert.DeserializeObject<List<PaymentType>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK,paymentTypeGetInitialResponse.StatusCode);
                var paymentTypeObject = paymentTypeList[0];

                var response = await client.GetAsync($"api/paymenttypes/{paymentTypeObject.Id}");

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypeReturned = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypeReturned.Id == paymentTypeObject.Id);
            }
        }

        [Fact]
        public async Task Test_Create_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType barclayCard = new PaymentType
                {
                    AcctNumber = 1234,
                    Name = "BarClay Credit",
                    CustomerId = 2
                };
                var barclayCardAsJson = JsonConvert.SerializeObject(barclayCard);

                var response = await client.PostAsync("api/paymenttypes",
                    new StringContent(barclayCardAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                var newBarclayCard = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1234, newBarclayCard.AcctNumber);
                Assert.Equal("BarClay Credit",newBarclayCard.Name);
                Assert.Equal(2, newBarclayCard.CustomerId);
            }
        }

        [Fact]
        public async Task Test_Modify_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                var paymentTypeGetInitialResponse = await client.GetAsync("api/paymenttypes");
                string initialResponseBody = await paymentTypeGetInitialResponse.Content.ReadAsStringAsync();
                var paymentTypeList = JsonConvert.DeserializeObject<List<PaymentType>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, paymentTypeGetInitialResponse.StatusCode);

                var paymentTypeObject = paymentTypeList[0];
                var defaultPaymentTypeName = paymentTypeObject.Name;

                //BEGIN PUT TEST
                paymentTypeObject.Name = "TestName";
                var modifiedPaymentTypeAsJson = JsonConvert.SerializeObject(paymentTypeObject);
                var response = await client.PutAsync($"api/paymenttypes/{paymentTypeObject.Id}",
                    new StringContent(modifiedPaymentTypeAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getPaymentType = await client.GetAsync($"api/paymenttypes/{paymentTypeObject.Id}");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                Assert.Equal("TestName", newPaymentType.Name);
                newPaymentType.Name = defaultPaymentTypeName;
                var returnPaymentTypeToDefault = JsonConvert.SerializeObject(newPaymentType);

                var putPaymentTypeToDefault = await client.PutAsync($"api/paymentTypes/{newPaymentType.Id}",
                    new StringContent(returnPaymentTypeToDefault, Encoding.UTF8, "application/json"));
                string originalPaymentTypeObject = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Remove_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var paymentTypeGetInitialResponse = await client.GetAsync("api/paymentTypes");
                string initialResponseBody = await paymentTypeGetInitialResponse.Content.ReadAsStringAsync();
                var paymentTypeList = JsonConvert.DeserializeObject<List<PaymentType>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, paymentTypeGetInitialResponse.StatusCode);
                int removeLastObject = paymentTypeList.Count - 1;
                var paymentTypeObject = paymentTypeList[removeLastObject];

                var response = await client.DeleteAsync($"api/paymenttypes/{paymentTypeObject.Id}");

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getPaymentType = await client.GetAsync($"api/paymenttypes/{paymentTypeObject.Id}");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                Assert.Equal(HttpStatusCode.NoContent, getPaymentType.StatusCode);
            }
        }
    }
}
