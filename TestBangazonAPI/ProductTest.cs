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
   public class ProductTest
    {
        [Fact]
        public async Task Test_GetAllProducts()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/products");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_GetSingleProduct()
        {

            using (var client = new APIClientProvider().Client)
            {
                int getThisId = 2;

                var response = await client.GetAsync($"api/products/{getThisId}");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productReturned = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productReturned.Id == getThisId);
            }
        }

        [Fact]
        public async Task Test_Create_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product ninSwitch = new Product
                {
                    ProductTypeId = 2,
                    CustomerId = 1,
                    Price = 250,
                    Title = "Switch",
                    Description = "Nes game sys",
                    Quantity = 1
                };
                var ninSwitchAsJson = JsonConvert.SerializeObject(ninSwitch);

                var response = await client.PostAsync("api/products",
                    new StringContent(ninSwitchAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                var newNinSwitch = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(2,newNinSwitch.ProductTypeId);
                Assert.Equal(1,newNinSwitch.CustomerId);
                Assert.Equal(250,newNinSwitch.Price);
                Assert.Equal("Switch",newNinSwitch.Title);
                Assert.Equal("Nes game sys",newNinSwitch.Description);
                Assert.Equal(1,newNinSwitch.Quantity);
            }
        }

        [Fact]
        public async Task Test_Modify_Product()
        {
            string newTitle = "Saturn";
            string newDesc = "Sega game sys";

            using (var client = new APIClientProvider().Client)
            {
                int alterThisId = 5;

                Product modifyProduct = new Product
                {
                    ProductTypeId = 2,
                    CustomerId = 1,
                    Price = 250,
                    Title = newTitle,
                    Description = newDesc,
                    Quantity = 1
                };

                var modifiedProductAsJson = JsonConvert.SerializeObject(modifyProduct);

                var response = await client.PutAsync($"api/products/{alterThisId}",
                    new StringContent(modifiedProductAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getProduct = await client.GetAsync($"api/products/{alterThisId}");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(newTitle, newProduct.Title);
                Assert.Equal(newDesc, newProduct.Description);
            }
        }

        [Fact]
        public async Task Test_Remove_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                int deleteThisId = 5;

                var response = await client.DeleteAsync($"api/products/{deleteThisId}");

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getProduct = await client.GetAsync($"api/products/{deleteThisId}");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.NoContent, getProduct.StatusCode);
            }
        }

    }
}
