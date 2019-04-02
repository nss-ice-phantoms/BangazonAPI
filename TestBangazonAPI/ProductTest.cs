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
                var productGetInitialResponse = await client.GetAsync("api/products");
                string initialResponseBody = await productGetInitialResponse.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK,productGetInitialResponse.StatusCode);
                var productObject = productList[0];

                //BEGIN GET SPECIFIC TESTING
                var response = await client.GetAsync($"api/products/{productObject.Id}");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productReturned = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productReturned.Id == productObject.Id);
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
            using (var client = new APIClientProvider().Client)
            {

                var productGetInitialResponse = await client.GetAsync("api/products");
                string initialResponseBody = await productGetInitialResponse.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, productGetInitialResponse.StatusCode);

                var productObject = productList[0];
                var defaultProductTitle = productObject.Title;

                //BEGIN PUT TEST
                productObject.Title = "ThisIsATest";
                var modifiedProductAsJson = JsonConvert.SerializeObject(productObject);
                var response = await client.PutAsync($"api/products/{productObject.Id}",
                    new StringContent(modifiedProductAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getProduct = await client.GetAsync($"api/products/{productObject.Id}");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);
                Assert.Equal("ThisIsATest", newProduct.Title);

                newProduct.Title = defaultProductTitle;
                var returnProductToDefault = JsonConvert.SerializeObject(newProduct);

                var putProductToDefault = await client.PutAsync($"api/products/{newProduct.Id}",
                    new StringContent(returnProductToDefault, Encoding.UTF8, "application/json"));
                string originalProductObject = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Remove_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                var productGetInitialResponse = await client.GetAsync("api/products");
                string initialResponseBody = await productGetInitialResponse.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, productGetInitialResponse.StatusCode);
                int removeLastObject = productList.Count - 1;
                var productObject = productList[removeLastObject];

                var response = await client.DeleteAsync($"api/products/{productObject.Id}");

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getProduct = await client.GetAsync($"api/products/{productObject.Id}");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.NoContent, getProduct.StatusCode);
            }
        }

    }
}
