using Microsoft.Playwright;
using NUnit.Allure.Core;
using System;
using NUnit.Framework.Internal;
using Newtonsoft.Json.Linq;

namespace newplaywright
{
	[TestFixture]
	[AllureNUnit]
	internal class GoRest
	{
		private IPlaywright playwright;
		private IAPIRequestContext context;

		[SetUp]
		public async Task setup() {
           playwright=await Playwright.CreateAsync();
			context = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
			{

				BaseURL = "https://gorest.co.in/public/v2/",
                

			}) ;

        }

		[Test]
		public async Task Users() {

			var response = await context.GetAsync("users");
			Assert.IsNotNull(response);
		}

        [Test]
        public async Task posts()
        {

            var response = await context.GetAsync("posts");
            Assert.IsNotNull(response);
        }


        [Test]
        public async Task comments()
        {

            var response = await context.GetAsync("comments");
            Assert.IsNotNull(response);
        }



        [Test]
        public async Task todos()
        {

            var response = await context.GetAsync("comments");
            Assert.IsNotNull(response);
        }


        [Test]
        public async Task PostData() {
         
            String AuthToken = "a6fc195dd60e618c4f0d37e15ae429917d090fe68d9ca16fd847681cddc448fa";
            var headers = new Dictionary<string, string> {
                ["Authorization"]=$"Bearer {AuthToken}"
            
            };

            var response = await context.PostAsync("users",new APIRequestContextOptions() { 
            
            DataObject=new {
                email="xz@4",
                name="Abc",
                gender="male",
                status= "Active"
            },

            Headers=headers

            });

            var data = await response.TextAsync();
            JObject obj =JObject.Parse(data);
            int id = (int)obj["id"];


           


            var put = await context.PutAsync($"users/{id}", new APIRequestContextOptions()
            {
                DataObject=new {
                email="mvc@123"
                
                },
                Headers = headers

            });

            var get = await context.GetAsync($"users/{id}", new APIRequestContextOptions()
            {
                Headers = headers

            });

            var getdata= await get.TextAsync();
            Console.WriteLine(getdata);



            var delete = await context.DeleteAsync($"users/{id}",new APIRequestContextOptions() { 
            Headers=headers
            });

           Assert.That((int)delete.Status, Is.EqualTo(204));

        
        }



    }
}