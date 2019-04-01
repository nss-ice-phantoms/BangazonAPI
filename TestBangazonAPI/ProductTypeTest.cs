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
                var response = await client.GetAsync("/api/producttype");

                string responseBody = await response.Content.ReadAsStringAsync();
                var typeList = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /* ASSERT */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(typeList.Count > 0);
            }
        }


        /*************
         * POST Test
         ************/
        [Fact]
        public async Task Test_Create_Product_Type()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*  ARRANGE */
                
                ProductType newType = new ProductType
                {
                    Name = "Health & Beauty"
                };
                
                var typeAsJSON = JsonConvert.SerializeObject(newType);


                /* ACT */
                
                var response = await client.PostAsync(
                    "/api/producttype",
                    new StringContent(typeAsJSON, Encoding.UTF8, "application/json")
                );
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var returnedType = JsonConvert.DeserializeObject<ProductType>(responseBody);


                /* ASSERT */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Health & Beauty", returnedType.Name);
            }
        }

        /*************
         * PUT Test
         ************/
        [Fact]
        public async Task Test_Modify_Product_Type()
        {
            // New cohort Id to change to and test
            string newName = "Sports";

            using (var client = new APIClientProvider().Client)
            {
                /* PUT section */
                ProductType modifiedType = new ProductType
                {
                    Name = newName
                };
                var modifiedStudentAsJSON = JsonConvert.SerializeObject(modifiedType);

                var response = await client.PutAsync(
                    "/api/producttype/2",
                    new StringContent(modifiedStudentAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /* GET section - Verify that the PUT operation was successful */
                var getType = await client.GetAsync("/api/producttype/2");
                getType.EnsureSuccessStatusCode();

                string getTypeBody = await getType.Content.ReadAsStringAsync();
                ProductType newType = JsonConvert.DeserializeObject<ProductType>(getTypeBody);

                Assert.Equal(HttpStatusCode.OK, getType.StatusCode);
                Assert.Equal(newName, newType.Name);
            }
        }

        /*************
         * DELETE Test
         ************/
        [Fact]
        public async Task Test_Delete_Product_Type()
        {

            using (var client = new APIClientProvider().Client)
            {
                /* DELETE section */
                var response = await client.DeleteAsync("/api/producttype/1005");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /* GET section - Verify that the DELETE operation was successful */
                var getType = await client.GetAsync("/api/producttype/1005");
                Assert.Equal(HttpStatusCode.NoContent, getType.StatusCode);
            }
        }

    }
}
