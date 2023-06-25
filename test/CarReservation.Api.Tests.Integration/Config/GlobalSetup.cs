using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace CarReservation.Api.Tests.Integration.Config
{
    public class GlobalSetup
    {
        protected TestServer TestServer;
        protected HttpClient HttpClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                });

            TestServer = factory.Server;
            HttpClient = factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() 
        {
            HttpClient?.Dispose();
            TestServer?.Dispose();
        }
    }
}
