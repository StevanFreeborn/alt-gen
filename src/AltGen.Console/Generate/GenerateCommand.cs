namespace AltGen.Console.Generate;

sealed class GenerateCommand(
  IAnsiConsole console,
  IFileSystem fileSystem,
  IAltGenService altGenService,
  IAppSettingsManager settingsManager
) : AsyncCommand<GenerateCommand.Settings>
{

  readonly IAnsiConsole _console = console;
  readonly IFileSystem _fileSystem = fileSystem;
  readonly IAltGenService _altGenService = altGenService;
  readonly IAppSettingsManager _settingsManager = settingsManager;

  public class Settings(IFileSystem fileSystem) : CommandSettings
  {
    readonly Dictionary<string, string> _imageTypes = new()
    {
      [".jpeg"] = "image/jpeg",
      [".jpg"] = "image/jpeg",
      [".png"] = "image/png"
    };

    readonly IFileSystem _fileSystem = fileSystem;

    [CommandOption("-p|--provider")]
    [Description("The provider to use for generating alt text.")]
    public string Provider { get; init; } = string.Empty;

    [CommandOption("-k|--key")]
    [Description("The key for the provider.")]
    public string Key { get; init; } = string.Empty;

    [CommandArgument(1, "<path>")]
    [Description("The path to the image to generate alt text for.")]
    public string Path { get; init; } = string.Empty;

    public string ContentType => _imageTypes[_fileSystem.Path.GetExtension(Path)];

    public override ValidationResult Validate()
    {
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
    var appSettings = await _settingsManager.GetAppSettingsAsync();
    var provider = settings.Provider;
    var key = settings.Key;

    if (string.IsNullOrWhiteSpace(provider))
    {
      var defaultProvider = appSettings.GetDefaultProvider()
        ?? throw new AltTextException("Please specify a provider or set a default provider.");
      provider = defaultProvider.Provider;
    }

    if (Providers.IsSupported(provider) is false)
    {
      throw new AltTextException($"The provider '{provider}' is not supported.");
    }

    if (string.IsNullOrWhiteSpace(key))
    {
      var selectedProvider = appSettings.GetProvider(provider)
        ?? throw new AltTextException("Please specify a key or set a default provider.");
      key = selectedProvider.Key;
    }


    var fileName = _fileSystem.Path.GetFileName(settings.Path);
    var image = await _fileSystem.File.ReadAllBytesAsync(settings.Path);

    var altText = await _altGenService.GenerateAltTextAsync(new(
      provider,
      key,
      fileName,
      image,
      settings.ContentType
    ));

    _console.MarkupLine($"[bold]{altText}[/]");

    return 0;
  }
}