namespace AltGen.API.Tests.Integration;

public class IntegrationTest : IClassFixture<AppFactory>
{
  protected MockHttpMessageHandler MockGeminiServiceHandler { get; } = new();
  protected WebApplicationFactory<Program> Factory { get; }
  protected HttpClient Client { get; }

  public IntegrationTest(AppFactory factory)
  {
    Factory = factory.WithWebHostBuilder(
      builder => builder.ConfigureTestServices(
        services => services.AddHttpClient<IGeminiService, GeminiService>()
          .ConfigurePrimaryHttpMessageHandler(() => MockGeminiServiceHandler)
      )
    );

    Client = Factory.CreateClient();
  }
}