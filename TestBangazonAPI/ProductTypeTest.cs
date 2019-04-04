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
    public class ProductTypeTest
    {
        /************
         * GET Test
         ***********/
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {

            using (var client = new APIClientProvider().Client)
            {
                /* ARRANGE */

                /* ACT */
                var response = await client.GetAsync("/api/productTypes");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /* ASSERT */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var productTypeGetInitialResponse = await client.GetAsync("api/productTypes");
                string initialResponseBody = await productTypeGetInitialResponse.Content.ReadAsStringAsync();
                var productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, productTypeGetInitialResponse.StatusCode);
                var productTypeObject = productTypeList[0];

                //BEGIN GET SPECIFIC TESTING
                var response = await client.GetAsync($"api/productTypes/{productTypeObject.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypeReturned = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypeReturned.Id == productTypeObject.Id);
            }
        }


        /*************
         * POST Test
         ************/
        [Fact]
        public async Task Test_Create_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*  ARRANGE */
                
                ProductType newProductType = new ProductType
                {
                    Name = "Health & Beauty"
                };
                
                var productTypeAsJSON = JsonConvert.SerializeObject(newProductType);


                /* ACT */
                
                var response = await client.PostAsync(
                    "/api/productTypes",
                    new StringContent(productTypeAsJSON, Encoding.UTF8, "application/json")
                );
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var returnedProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Health & Beauty", returnedProductType.Name);
            }
        }


        /*************
         * PUT Test
         ************/
        [Fact]
        public async Task Test_Modify_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var productTypeGetInitialResponse = await client.GetAsync("api/productTypes");
                string initialResponseBody = await productTypeGetInitialResponse.Content.ReadAsStringAsync();
                var productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, productTypeGetInitialResponse.StatusCode);
                
                var productTypeObject = productTypeList[0];
                var defaultProductTypeName = productTypeObject.Name;

                /* PUT section */
                productTypeObject.Name = "ThisIsATest";

                var modifiedProductTypeAsJson = JsonConvert.SerializeObject(productTypeObject);
                var response = await client.PutAsync($"api/productTypes/{productTypeObject.Id}",
                    new StringContent(modifiedProductTypeAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var getProductType = await client.GetAsync($"api/productTypes/{productTypeObject.Id}");
                getProductType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getProductType.Content.ReadAsStringAsync();
                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);
                Assert.Equal("ThisIsATest", newProductType.Name);

                newProductType.Name = defaultProductTypeName;
                var returnProductTypeToDefault = JsonConvert.SerializeObject(newProductType);

                var putProductTypeToDefault = await client.PutAsync($"api/productTypes/{newProductType.Id}",
                    new StringContent(returnProductTypeToDefault, Encoding.UTF8, "application/json"));

                string originalProductTypeObject = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                
            }
        }

        /*************
         * DELETE Test
         ************/
        [Fact]
        public async Task Test_Remove_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var productTypeGetInitialResponse = await client.GetAsync("api/productTypes");
                string initialResponseBody = await productTypeGetInitialResponse.Content.ReadAsStringAsync();
                var productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(initialResponseBody);
                Assert.Equal(HttpStatusCode.OK, productTypeGetInitialResponse.StatusCode);

                int removeLastObject = productTypeList.Count - 1;
                var productTypeObject = productTypeList[removeLastObject];
                
                var response = await client.DeleteAsync($"api/productTypes/{productTypeObject.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                
                var getProductType = await client.GetAsync($"api/productTypes/{productTypeObject.Id}");
                getProductType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getProductType.Content.ReadAsStringAsync();
                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);
                
                Assert.Equal(HttpStatusCode.NoContent, getProductType.StatusCode);
            }

        }

    }
}
