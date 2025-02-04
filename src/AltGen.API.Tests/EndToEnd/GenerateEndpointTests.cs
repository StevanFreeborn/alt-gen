

namespace AltGen.API.Tests.EndToEnd;

public class GenerateEndpointTests(
  AppFactory factory,
  TestConfiguration config
) : EndToEndTest(factory, config)
{
  [Fact]
  public async Task GenerateEndpoint_WhenCalled_ItShouldReturnOk()
  {
    var fileName = "library.jpg";
    var file = await TestFileManager.GetFileAsync(fileName);
    var byteContent = new ByteArrayContent(file);
    byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

    var content = new MultipartFormDataContent()
    {
      { new StringContent("Gemini"), "Provider" },
      { new StringContent(Config.GeminiApiKey), "ProviderKey" },
      { byteContent, "File", fileName }
    };

    var response = await Client.PostAsync("/generate", content);
    var altText = await response.Content.ReadFromJsonAsync<GenerateResponse>();

    response.Should().HaveStatusCode(HttpStatusCode.OK);
    altText!.AltText.Should().NotBeNullOrWhiteSpace();
  }
}