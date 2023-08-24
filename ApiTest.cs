using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using NUnit.Allure.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace newplaywright
{
    [TestFixture]
    [AllureNUnit]
    internal class ApiTest
    {
        private IPlaywright playwright;
        private IAPIRequestContext context;

        [SetUp]
        public async Task setup() { 
        playwright=await Playwright.CreateAsync();  
        context=await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
        { 
            BaseURL= "https://petstore.swagger.io/v2/"

        });
     
        }

        [Test]
        public async Task getByUserName() {
            var name = "Shobhit";
            var response = await context.GetAsync($"user/{name}");
            Assert.AreEqual(404,(int)response.Status);
        
        }

        [Test]
        public async Task getUserByLogin() {
           var response =await context.GetAsync("user/login?username=sd%40123&password=12345");
            Assert.AreEqual(200, response.Status);


        }

        [Test]
        public async Task getUserByLogout() {
            var response = await context.GetAsync("user/logout");
            Assert.AreEqual(200, response.Status);

        }


        [Test]
        public async Task createUser() {
            String dataSend = "\r\n  \"id\": 0,\r\n  \"username\": \"string\",\r\n  \"firstName\": \"string\",\r\n  \"lastName\": \"string\",\r\n  \"email\": \"string\",\r\n  \"password\": \"string\",\r\n  \"phone\": \"string\",\r\n  \"userStatus\": 0";

            var response = await context.PostAsync("user", new APIRequestContextOptions()
            {
                DataObject=new
                {
                    dataSend
                }

            });

            String data = await response.TextAsync();
            Console.WriteLine(data);
            JObject obj = JObject.Parse(data);
  
        }
        [TearDown]
        public void teardown() {
         context.DisposeAsync();
        
        
        }

    }
}
