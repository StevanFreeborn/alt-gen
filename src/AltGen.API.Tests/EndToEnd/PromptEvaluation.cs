namespace AltGen.API.Tests.EndToEnd;

public class PromptEvaluations(
  AppFactory factory,
  TestConfiguration config
) : EndToEndTest(factory, config)
{
  // TODO: Use LLM-assisted prompt evaluation instead of
  // doing partial string matching. Better suited due to
  // subjective nature of the task.
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Generate_WhenCalled_ItShouldRespondWithAltTextContainingKeyWords(string imageName, string[] keyWords)
  {
    var file = await TestFileManager.GetFileAsync(imageName);
    var byteContent = new ByteArrayContent(file);
    byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

    var content = new MultipartFormDataContent()
    {
      { new StringContent("Gemini"), "Provider" },
      { new StringContent(Config.GeminiApiKey), "ProviderKey" },
      { byteContent, "File", imageName }
    };

    var response = await Client.PostAsync("/generate", content);
    var altText = await response.Content.ReadFromJsonAsync<GenerateResponse>();

    response.Should().HaveStatusCode(HttpStatusCode.OK);
    altText!.AltText.ToLowerInvariant().Should().ContainAll(keyWords);
  }

  class TestData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      yield return new object[] { "library.jpg", new[] { "book", } };
      yield return new object[] { "ascii_art.jpg", new[] { "test", "100", "success", } };
      yield return new object[] { "living_room.jpg", new[] { "blue", "dog", } };
      yield return new object[] { "podcast.jpg", new[] { "podcast", "scott", "mark", } };
      yield return new object[] { "youtube_comment.jpg", new[] { "feedback", } };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}