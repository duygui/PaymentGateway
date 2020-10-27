using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PaymentGatewayTests
{
    public class APIWebApplicationFactory : WebApplicationFactory<PaymentGatewayTests.TestStartup>
    {
    }

    [TestFixture]
    public class Tests
    {
        //public TestServer CreateServer()
        //{
        //    var path = Assembly.GetAssembly(typeof(Tests))
        //       .Location;

        //    var hostBuilder = new WebHostBuilder()
        //        .UseContentRoot(Path.GetDirectoryName(path))
        //        .ConfigureAppConfiguration(cb =>
        //        {
        //            cb.AddJsonFile("appsettings.json", optional: false)
        //            .AddEnvironmentVariables();
        //        }).UseStartup<TestStartup>();

        //    return new TestServer(hostBuilder);
        //}


        //[Test]
        //public async Task GetPaymentStatus()
        //{
        //    using (var server = CreateServer())
        //    {
        //        var client =server.CreateClient();
        //        var res = await client.GetAsync("payment?trackingNumber=262e2c75-865a-4037-9405-3f91bef11a50");
        //    }

        //}


        private APIWebApplicationFactory _factory;
        private System.Net.Http.HttpClient _client;

        [OneTimeSetUp]
        public void GivenARequestToTheController()
        {
            _factory = new APIWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task WhenSomeTextIsPosted_ThenTheResultIsOk()
        {
          
            var result = await _client.GetAsync("/payments?trackingNumber=262e2c75-865a-4037-9405-3f91bef11a50");
            Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        //[Test]
        //public async Task WhenNoTextIsPosted_ThenTheResultIsBadRequest()
        //{
        //    var result = await _client.PostAsync("/sample", new StringContent(string.Empty));
        //    Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        //}

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }



        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}