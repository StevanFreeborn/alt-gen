using System.Net;

using FluentAssertions;
namespace AltGen.Console.Tests.Unit;

public class AltGenServiceTests : IDisposable
{
  readonly MockHttpMessageHandler _mockHttpMessageHandler = new();
  readonly AltGenService _altGenService;

  public AltGenServiceTests()
  {
    _altGenService = new AltGenService(_mockHttpMessageHandler.ToHttpClient());
  }

  [Fact]
  public async Task GenerateAltTextAsync_WhenCalledAndRequestSucceeds_ItShouldReturnAltText()
  {
    var req = new GenerateAltTextRequest(
      "provider",
      "key",
      "file.jpg",
      [1, 2, 3],
      "image/jpeg"
    );

    var altText = "alt text";

    _mockHttpMessageHandler
      .When(HttpMethod.Post, "*/generate")
      .Respond(
        "application/json",
        /*lang=json,strict*/
        $@"{{""AltText"":""{altText}""}}"
      );

    var result = await _altGenService.GenerateAltTextAsync(req);

    result.Should().Be(altText);
  }

  [Fact]
  public async Task GenerateAltTextAsync_WhenCalledAndRequestFails_ItShouldThrowException()
  {
    var req = new GenerateAltTextRequest(
      "provider",
      "key",
      "file.jpg",
      [1, 2, 3],
      "image/jpeg"
    );

    _mockHttpMessageHandler
      .When(HttpMethod.Post, "*/generate")
      .Respond(HttpStatusCode.InternalServerError);

    var action = async () => await _altGenService.GenerateAltTextAsync(req);

    await action.Should().ThrowAsync<AltTextException>();
  }

  [Fact]
  public async Task GenerateAltTextAsync_WhenCalledAndResponseIsNull_ItShouldThrowException()
  {
    var req = new GenerateAltTextRequest(
      "provider",
      "key",
      "file.jpg",
      [1, 2, 3],
      "image/jpeg"
    );

    _mockHttpMessageHandler
      .When(HttpMethod.Post, "*/generate")
      .Respond("application/json", "null");

    var action = async () => await _altGenService.GenerateAltTextAsync(req);

    await action.Should().ThrowAsync<AltTextException>();
  }

  public void Dispose()
  {
    _mockHttpMessageHandler.Dispose();
    GC.SuppressFinalize(this);
  }
}