using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

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
                    //builder.ConfigureAppConfiguration(config =>
                    //{
                    //    config.AddInMemoryCollection(new Dictionary<string, string?>
                    //    {
                    //        ["ConnectionStrings:MongoDb"] = _mongoDb.GetConnectionString(),
                    //        ["ConnectionStrings:AzureStorage"] = _azurite.GetConnectionString(),
                    //    });
                    //});
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
