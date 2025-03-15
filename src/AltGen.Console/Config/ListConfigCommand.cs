
namespace AltGen.Console.Config;

sealed class ListConfigCommand(
  IAnsiConsole console,
  IAppSettingsManager settingsManager
) : AsyncCommand
{
  readonly IAnsiConsole _console = console;
  readonly IAppSettingsManager _settingsManager = settingsManager;

  public override async Task<int> ExecuteAsync(CommandContext context)
  {
    if (_settingsManager.AppSettingsExist() is false)
    {
      _console.MarkupLine("No settings found.");
      return 0;
    }

    var appSettings = await _settingsManager.GetAppSettingsAsync();

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