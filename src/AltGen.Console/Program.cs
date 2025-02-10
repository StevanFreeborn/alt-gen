// TODO: We want to be able
// to provide a path to an image
// read the image
// and then post the image to our API
// get the response and display the alt text

using System.ComponentModel;
using System.IO.Abstractions;
using System.Net.Http.Headers;

using AltGen.Console.Common;

using Spectre.Console;

await Host.CreateDefaultBuilder(args)
  .ConfigureServices(static (_, services) =>
  {
    services.AddSingleton<IFileSystem, FileSystem>();
    services.AddSingleton(AnsiConsole.Console);
    services.AddHttpClient<IAltGenService, AltGenService>()
      .AddStandardResilienceHandler();
  })
  .BuildApp()
  .RunAsync(args);

sealed class GenerateCommand(
  IAnsiConsole console,
  IFileSystem fileSystem,
  IAltGenService altGenService
) : AsyncCommand<GenerateCommand.Settings>
{

  readonly IAnsiConsole _console = console;
  readonly IFileSystem _fileSystem = fileSystem;
  readonly IAltGenService _altGenService = altGenService;

  public class Settings(IFileSystem fileSystem) : CommandSettings
  {
    readonly Dictionary<string, string> _imageTypes = new()
    {
      [".jpeg"] = "image/jpeg",
      [".jpg"] = "image/jpeg",
      [".png"] = "image/png"
    };

    readonly List<string> _providers = [
      "gemini",
    ];

    readonly IFileSystem _fileSystem = fileSystem;

    [CommandArgument(1, "<provider>")]
    [Description("The provider to use for generating alt text.")]
    public string Provider { get; init; } = string.Empty;

    [CommandArgument(2, "<key>")]
    [Description("The key for the provider.")]
    public string Key { get; init; } = string.Empty;

    [CommandArgument(3, "<path>")]
    [Description("The path to the image to generate alt text for.")]
    public string Path { get; init; } = string.Empty;

    public string ContentType => _imageTypes[_fileSystem.Path.GetExtension(Path)];

    public override ValidationResult Validate()
    {
      if (_providers.Contains(Provider) is false)
      {
        return ValidationResult.Error($"The provider '{Provider}' is not supported.");
      }

      var pathExists = _fileSystem.File.Exists(Path);

      if (pathExists is false)
      {
        return ValidationResult.Error($"The path '{Path}' does not exist.");
      }

      var extension = _fileSystem.Path.GetExtension(Path);

      if (_imageTypes.ContainsKey(extension) is false)
      {
        return ValidationResult.Error($"The file '{Path}' is not a valid image file.");
      }

      return ValidationResult.Success();
    }
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    var fileName = _fileSystem.Path.GetFileName(settings.Path);
    var image = await _fileSystem.File.ReadAllBytesAsync(settings.Path);

    var altText = await _altGenService.GenerateAltTextAsync(
      settings.Provider,
      settings.Key,
      fileName,
      image,
      settings.ContentType
    );

    _console.MarkupLine($"[bold]Alt Text:[/] {altText}");

    return 0;
  }
}

interface IAltGenService
{
  Task<string> GenerateAltTextAsync(
    string provider,
    string key,
    string fileName,
    byte[] image,
    string contentType
  );
}

sealed class AltGenService(HttpClient client) : IAltGenService
{
  readonly HttpClient _client = client;

  public async Task<string> GenerateAltTextAsync(
    string provider,
    string key,
    string fileName,
    byte[] image,
    string contentType
  )
  {
    var byteContent = new ByteArrayContent(image);
    byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

    // TODO: Need to have proper URL
    // have constants generated at build time
    // that point to proper API URL
    var request = new HttpRequestMessage(HttpMethod.Post, "altgen")
    {
      Content = new MultipartFormDataContent
      {
        { new StringContent(provider), "provider" },
        { new StringContent(key), "providerKey" },
        { new ByteArrayContent(image), "file", fileName }
      }
    };

    var response = await _client.SendAsync(request);

    if (response.IsSuccessStatusCode is false)
    {
      // TODO: Use accurate exception
      throw new InvalidDataException("Failed to generate alt text.");
    }

    // TODO: Deserialize the response
    var altText = await response.Content.ReadAsStringAsync();

    return altText;
  }
}