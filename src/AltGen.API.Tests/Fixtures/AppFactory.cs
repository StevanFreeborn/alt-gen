using Microsoft.Extensions.Logging;

namespace AltGen.API.Tests.Fixtures;

public class AppFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureLogging(static l => l.ClearProviders());
  }
}