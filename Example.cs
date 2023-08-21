
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Allure.Core;
using NUnit.Framework;
using NUnit.Framework.Internal;
//using NUnit.Framework.Internal.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;


namespace newplaywright
{
    [AllureNUnit]
    internal class Example
    {

        private Serilog.ILogger log;
      
        

        [OneTimeSetUp]
        public Task Setup()
        {
            log = new LoggerConfiguration()
                   .WriteTo.Console()
                   .CreateLogger();
           
        }

        [Test]
        [Category("crud")]
        public async Task Test1() {

            var playwright = await Playwright.CreateAsync();
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://reqres.in/"

            });
            var response = await request.GetAsync("api/users?page=2");
            var data=await response.TextAsync();
          
            log.Information("Hello");
        
        }

        [Test]
        public async Task  Test2() { 
           var playwright=await Playwright.CreateAsync();
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://reqres.in/"

            });

            var response = await request.PostAsync("api/users",new APIRequestContextOptions()
            {
              DataObject = new
              {
                  name = "morpheus",
                  job = "leader"
              }

            });


            var data = await response.TextAsync();
          //  Console.WriteLine(data);

        
        }

        [Test]
        public async Task Test3() { 
        
            var playwright=await Playwright.CreateAsync();
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions(){
                BaseURL = "https://reqres.in/"
            });

            var response = await request.DeleteAsync("api/users/2");
            var data =await response.TextAsync();
            Console.WriteLine($"response: {response.Status}");
        }

        [Test]
        public async Task Test4() { 
        
        var playwright=await Playwright.CreateAsync();
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://reqres.in/"
            });

            var response = await request.GetAsync("api/users/2");
            String data = await response.TextAsync();
            Console.WriteLine($"Response Status: {response.Status}");
           // var jsonobj = System.Text.Json.JsonDocument.Parse(data).RootElement;
          //  dynamic obj=JsonConvert.DeserializeObject(data);
          JObject obj=JObject.Parse(data);  
          String email = (String)obj["first_name"];

            // Assert.
       //   Console.WriteLine(email);  
         //  Console.WriteLine(data);

        }

        [Test] 
        public async Task Post() {
            var playwriter = await Playwright.CreateAsync();
            var request = await playwriter.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
                { 
           BaseURL= "https://petstore.swagger.io/"
            });



            var response = await request.GetAsync("v2/pet/findByStatus?status=available");

            var data = await response.TextAsync();
            
           // Console.WriteLine(data);
            var headers = response.Headers;
            foreach (var header in headers) {
                Console.WriteLine($"Header {header.Key}={header.Value}");
            
            }
            var cookies = response.Headers.Where(header => header.Key.Equals("Set-Cookie"));
            foreach (var cookie in cookies)
            {
                Console.WriteLine($"Set-Cookie:{cookie.Value}");
            }
        }

        [Test]
        public async Task Test7() { 
        var playwright=await Playwright.CreateAsync();
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://reqres.in/"

            }) ;

        
        }

        [Test]
        public async Task Test5()
        {

            var playwright = await Playwright.CreateAsync();
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://reqres.in/"
            });

            var response = await request.GetAsync("api/users");
            String data = await response.TextAsync();
          //  Console.WriteLine(data);
            Console.WriteLine($"Response Status: {response.Status}");
            //  dynamic obj=JsonConvert.DeserializeObject(data);


            JObject obj = JObject.Parse(data);
            JArray dataArray = (JArray)obj["data"];

            JObject user = (JObject)dataArray[0];
            // Assert.
            Console.WriteLine(user);

            Console.WriteLine(user["id"]);

            Assert.AreEqual(1, (int)user["id"], "values are not equale");
            Assert.AreEqual("George", (String)user["first_name"],"name is not matching");
            
        }

        [Test]
        public async Task AuthTest() {
            const String AuthToken = "a6fc195dd60e618c4f0d37e15ae429917d090fe68d9ca16fd847681cddc448fa";
            var playwright = await Playwright.CreateAsync();
            
            var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://gorest.co.in/"
            });

            var headers = new Dictionary<String, String> {
                ["Authorization"]=$"Bearer {AuthToken}"
            };
            var response = await request.PostAsync("/public/v2/users/2627/posts", new APIRequestContextOptions()
            {
                DataObject = new
                {
                    title = "Shobhit",
                    body = "Automation"

                },

                Headers = headers
            }) ;

            Console.WriteLine(response.Status);
           

        }


    }
}
