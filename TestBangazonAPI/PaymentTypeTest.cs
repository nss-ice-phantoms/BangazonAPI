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
                int getThisId = 2;

                var response = await client.GetAsync($"api/paymenttypes/{getThisId}");

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypeReturned = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypeReturned.Id == getThisId);
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
            string newName = "Apple Credit";
            int newAcct = 1243;

            using (var client = new APIClientProvider().Client)
            {
                int alterThisId = 5;

                PaymentType modifyPaymentType = new PaymentType
                {
                    AcctNumber = newAcct,
                    Name = newName,
                    CustomerId = 2
                };

                var modifiedPaymentTypeAsJson = JsonConvert.SerializeObject(modifyPaymentType);

                var response = await client.PutAsync($"api/paymenttypes/{alterThisId}",
                    new StringContent(modifiedPaymentTypeAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getPaymentType = await client.GetAsync($"api/paymenttypes/{alterThisId}");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                Assert.Equal(newName, newPaymentType.Name);
                Assert.Equal(newAcct, newPaymentType.AcctNumber);
            }
        }

        [Fact]
        public async Task Test_Remove_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                int deleteThisId = 5;

                var response = await client.DeleteAsync($"api/paymenttypes/{deleteThisId}");

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getPaymentType = await client.GetAsync($"api/paymenttypes/{deleteThisId}");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                Assert.Equal(HttpStatusCode.NoContent, getPaymentType.StatusCode);
            }
        }
    }
}
