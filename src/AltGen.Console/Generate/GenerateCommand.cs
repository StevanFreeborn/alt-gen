namespace AltGen.Console.Generate;

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
      // TODO: We should allow provider and key to be optional
      // if they are not passed on the command line then
      // we should try to resolve them from configuration
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

    var altText = await _altGenService.GenerateAltTextAsync(new(
      settings.Provider,
      settings.Key,
      fileName,
      image,
      settings.ContentType
    ));

    _console.MarkupLine($"[bold]{altText}[/]");

    return 0;
  }
}