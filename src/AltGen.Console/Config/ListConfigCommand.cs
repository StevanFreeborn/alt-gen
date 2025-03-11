
namespace AltGen.Console.Config;

sealed class ListConfigCommand(IAnsiConsole console, IFileSystem fileSystem) : AsyncCommand
{
  readonly IAnsiConsole _console = console;
  readonly IFileSystem _fileSystem = fileSystem;

  public override async Task<int> ExecuteAsync(CommandContext context)
  {
    var settingsPath = _fileSystem.Path.Combine(AppContext.BaseDirectory, AppSettings.SettingsFileName);
    var settingsExist = _fileSystem.File.Exists(settingsPath);

    if (settingsExist is false)
    {
      _console.MarkupLine("No settings found.");
      return 0;
    }

    var settingsJson = await _fileSystem.File.ReadAllTextAsync(settingsPath);
    var appSettings = JsonSerializer.Deserialize<AppSettings>(settingsJson, JsonOptions.Default)
      ?? throw new ConfigException("Failed to deserialize settings.");

    foreach (var provider in appSettings.Providers)
    {
      var providerName = provider.Provider;
      var providerKey = provider.Key;
      var isDefault = provider.Default ? " (default)" : string.Empty;
      _console.MarkupLine($"[bold]{providerName}[/] [dim]{providerKey}[/]{isDefault}");
    }

    return 0;
  }
}