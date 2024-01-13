using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.TestMisc
{
    public class TestFixture : IDisposable
    {
        public readonly TestServer Server;
        private readonly HttpClient _client;

        public TestFixture(Action<IWebHostBuilder> configureWebHost)
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseContentRoot($"..\\GoodBadHabitsTracker.API\\");
                    builder.UseEnvironment("Development");
                    builder.UseTestServer();
                    configureWebHost(builder);
                });

            Server = factory.Server;
            _client = factory.CreateClient();
        }
        public void Dispose()
        {
            _client.Dispose();
            Server.Dispose();
        }
    }
}
