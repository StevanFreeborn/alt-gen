namespace AltGen.Console.Config;

sealed class RemoveConfigCommand(
  IFileSystem fileSystem,
  IAnsiConsole console
) : AsyncCommand<RemoveConfigCommand.Settings>
{
  readonly IFileSystem _fileSystem = fileSystem;
  readonly IAnsiConsole _console = console;

  public class Settings(IFileSystem fileSystem) : CommandSettings
  {
    readonly IFileSystem _fileSystem = fileSystem;

    [CommandArgument(1, "<provider>")]
    [Description("The provider to remove.")]
    public string Provider { get; init; } = string.Empty;

    public string SettingsPath => _fileSystem.Path.Combine(AppContext.BaseDirectory, "appsettings.json");
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    var exists = _fileSystem.Path.Exists(settings.SettingsPath);

    if (exists is false)
    {
      _console.MarkupLine("[bold]No existing settings found.[/]");
      return 1;
    }

    var settingsJson = await _fileSystem.File.ReadAllTextAsync(settings.SettingsPath);
    var appSettings = JsonSerializer.Deserialize<AppSettings>(settingsJson) ?? throw new ConfigException("Failed to deserialize app settings.");
    var updatedAppSettings = appSettings.RemoveProvider(settings);
    var updatedJson = JsonSerializer.Serialize(updatedAppSettings);
    await _fileSystem.File.WriteAllTextAsync(settings.SettingsPath, updatedJson);
    _console.MarkupLine($"[bold]{settings.Provider}[/] has been removed.");
    return 0;
  }
}