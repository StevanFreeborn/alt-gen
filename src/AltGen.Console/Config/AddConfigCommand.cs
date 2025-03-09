namespace AltGen.Console.Config;

sealed class AddConfigCommand(
  IAnsiConsole console,
  IFileSystem fileSystem
) : AsyncCommand<AddConfigCommand.Settings>
{
  static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
  };
  readonly IAnsiConsole _console = console;
  readonly IFileSystem _fileSystem = fileSystem;

  public class Settings(IFileSystem filesystem) : CommandSettings
  {
    readonly IFileSystem _fileSystem = filesystem;

    [CommandArgument(1, "<provider>")]
    [Description("The provider to configure.")]
    public string Provider { get; init; } = string.Empty;

    [CommandArgument(2, "<key>")]
    [Description("The key for the provider.")]
    public string Key { get; init; } = string.Empty;

    [CommandOption("-d|--default")]
    [Description("Set the provider as the default.")]
    public bool Default { get; init; }

    public string SettingsPath => _fileSystem.Path.Combine(AppContext.BaseDirectory, "appsettings.json");
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    var settingsExist = _fileSystem.File.Exists(settings.SettingsPath);

    if (settingsExist is false)
    {
      var appSettings = new AppSettings([
        new(settings.Provider, settings.Key, settings.Default)
      ]);

      var json = JsonSerializer.Serialize(appSettings, JsonOptions);
      await _fileSystem.File.WriteAllTextAsync(settings.SettingsPath, json);
      _console.MarkupLine($"[bold]{settings.Provider}[/] has been configured.");
      return 0;
    }

    var existingJson = await _fileSystem.File.ReadAllTextAsync(settings.SettingsPath);
    var existingAppSettings = JsonSerializer.Deserialize<AppSettings>(existingJson, JsonOptions)
      ?? throw new ConfigException("Failed to deserialize settings.");
    var updatedAppSettings = existingAppSettings.AddOrUpdateProvider(settings);
    var udpatedJson = JsonSerializer.Serialize(updatedAppSettings, JsonOptions);
    await _fileSystem.File.WriteAllTextAsync(settings.SettingsPath, udpatedJson);
    _console.MarkupLine($"[bold]{settings.Provider}[/] has been configured.");
    return 0;
  }
}