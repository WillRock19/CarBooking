using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Integration.Config
{
    public class GlobalSetup
    {
        protected TestServer GlobalTestServer;
        protected HttpClient GlobalHttpClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                });

            GlobalTestServer = factory.Server;
            GlobalHttpClient = factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() 
        {
            GlobalHttpClient?.Dispose();
            GlobalTestServer?.Dispose();
        }
    }
}
