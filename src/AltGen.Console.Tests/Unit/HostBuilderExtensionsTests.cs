namespace AltGen.Console.Tests.Unit;

public class HostBuilderExtensionsTests
{
  [Fact]
  public void BuildApp_WhenCalled_ItShouldBuildApp()
  {
    var hostBuilder = new HostBuilder();

    var app = hostBuilder.BuildApp();

    app.Should().NotBeNull();
  }
}