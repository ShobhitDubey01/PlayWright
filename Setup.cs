using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Serilog;
using System;

public class Class1 : PlaywrightTest
{
    private Serilog.ILogger log;
    private IAPIRequestContext? request;


    [OneTimeSetUp]
    public async Task setup()
    {
        log = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

        //request = await Playwright.APIRequest.NewContextAsync(new ()
        //{
        //    BaseURL = "https://reqres.in/"

        //});

    }


    [Test]
    [Category("crud")]
    public async Task test1()
    {  
       
        var response = await request.GetAsync("https://reqres.in/api/users?page=2");
        var data = await response.TextAsync();

        log.Information("Hello");

    }
}
