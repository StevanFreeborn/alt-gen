namespace AltGen.API.Tests.Integration;

public class GenerateEndpointTests : IntegrationTest
{
  const string TestImageName = "library.jpg";

  public GenerateEndpointTests(AppFactory factory) : base(factory)
  {
    MockGeminiServiceHandler.Clear();
  }

  [Theory]
  [ClassData(typeof(InvalidRequestTestData))]
  public async Task GenerateEndpoint_WhenCalledWithoutInvalidParameters_ItShouldReturnAProblemDetailWithStatusCode400(MultipartFormDataContent content, Dictionary<string, string[]> errors)
  {
    var response = await Client.PostAsync("/generate", content);
    var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

    response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    problem!.Errors.Should().BeEquivalentTo(errors);
  }

  [Theory]
  [ClassData(typeof(ValidRequestTestData))]
  public async Task GenerateEndpoint_WhenCalledWithRequiredParameters_ItShouldReturnOkWithAltText(MultipartFormDataContent content)
  {
    MockGeminiServiceHandler
      .When(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent/?key=ProviderKey")
      .Respond(
        "application/json",
        /*lang=json,strict*/
        @"{
            ""candidates"": [
              {
                ""content"": {
                  ""parts"": [
                    { ""text"": ""A long, perspective view of a classic library interior showcases richly colored wooden bookshelves filled with antique books, creating an atmosphere of history and scholarship.\n""
                    }
                  ],
                  ""role"": ""model""
                },
                ""finishReason"": ""STOP"",
                ""avgLogprobs"": -0.3317602475484212
              }
            ],
            ""usageMetadata"": {
              ""promptTokenCount"": 405,
              ""candidatesTokenCount"": 30,
              ""totalTokenCount"": 435,
              ""promptTokensDetails"": [
                  {
                    ""modality"": ""TEXT"",
                    ""tokenCount"": 147
                  },
                  {
                    ""modality"": ""IMAGE"",
                    ""tokenCount"": 258
                  }
                ],
              ""candidatesTokensDetails"": [
                {
                  ""modality"": ""TEXT"",
                  ""tokenCount"": 30
                }
              ]
            },
            ""modelVersion"": ""gemini-1.5-flash""
          }"
      );

    var response = await Client.PostAsync("/generate", content);
    var altText = await response.Content.ReadFromJsonAsync<GenerateResponse>();

    response.Should().HaveStatusCode(HttpStatusCode.OK);
    altText!.AltText.Should().NotBeNullOrWhiteSpace();
  }

  class ValidRequestTestData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      var testImage = TestFileManager.GetFile(TestImageName);
      var byteContent = new ByteArrayContent(testImage);
      byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent("Gemini"), "Provider" },
          { new StringContent("ProviderKey"), "ProviderKey" },
          { byteContent, "File", TestImageName }
        }
      };

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent("Gemini"), "Provider" },
          { new StringContent("ProviderKey"), "ProviderKey" },
          { byteContent, "File", TestImageName }
        }
      };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  class InvalidRequestTestData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent(string.Empty), "Provider" },
          { new StringContent(string.Empty), "ProviderKey" },
          { new ByteArrayContent([]), "File", "file.jpeg" }
        },
        new Dictionary<string, string[]>
        {
          { "Provider", ["The Provider field is required."] },
          { "ProviderKey", ["The ProviderKey field is required."] },
          { "File", ["The File has no content."] }
        },
      };

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent("Gemini"), "Provider" },
          { new StringContent(string.Empty), "ProviderKey" },
          { new ByteArrayContent([]), "File", "file.jpeg" }
        },
        new Dictionary<string, string[]>
        {
          { "ProviderKey", ["The ProviderKey field is required."] },
          { "File", ["The File has no content."] }
        },
      };

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent(string.Empty), "Provider" },
          { new StringContent("ProviderKey"), "ProviderKey" },
          { new ByteArrayContent([]), "File", "file.jpeg" }
        },
        new Dictionary<string, string[]>
        {
          { "Provider", ["The Provider field is required."] },
          { "File", ["The File has no content."] }
        },
      };

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent("Gemini"), "Provider" },
          { new StringContent("ProviderKey"), "ProviderKey" },
          { new ByteArrayContent([]), "File", "file.jpeg" }
        },
        new Dictionary<string, string[]>
        {
          { "File", ["The File has no content."] }
        },
      };

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent("MadeUp"), "Provider" },
          { new StringContent("ProviderKey"), "ProviderKey" },
          { new ByteArrayContent(Encoding.UTF8.GetBytes("Hello, World!")), "File", "file.jpeg" }
        },
        new Dictionary<string, string[]>()
        {
          { "Provider", ["The Provider field is invalid."] }
        },
      };

      yield return new object[]
      {
        new MultipartFormDataContent()
        {
          { new StringContent("Gemini"), "Provider" },
          { new StringContent("ProviderKey"), "ProviderKey" },
          { new ByteArrayContent(Encoding.UTF8.GetBytes("Hello, World!")), "File", "file.txt" }
        },
        new Dictionary<string, string[]>()
        {
          { "File", ["The File must be a valid image file."] }
        },
      };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}