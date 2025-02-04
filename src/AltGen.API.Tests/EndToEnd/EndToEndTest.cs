namespace AltGen.API.Tests.EndToEnd;

public class EndToEndTest(AppFactory factory, TestConfiguration config) : IClassFixture<AppFactory>, IClassFixture<TestConfiguration>
{
  protected HttpClient Client { get; } = factory.CreateClient();
  protected TestConfiguration Config { get; } = config;
}