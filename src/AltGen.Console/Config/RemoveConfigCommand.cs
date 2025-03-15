namespace AltGen.Console.Config;

sealed class RemoveConfigCommand(
  IAnsiConsole console,
  IAppSettingsManager settingsManager
) : AsyncCommand<RemoveConfigCommand.Settings>
{
  readonly IAnsiConsole _console = console;
  readonly IAppSettingsManager _settingsManager = settingsManager;

  public class Settings : CommandSettings
  {
    [CommandArgument(1, "<provider>")]
    [Description("The provider to remove.")]
    public string Provider { get; init; } = string.Empty;
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    if (_settingsManager.AppSettingsExist() is false)
    {
      throw new ConfigException("No existing settings found.");
    }

    var appSettings = await _settingsManager.GetAppSettingsAsync();
    var updatedAppSettings = appSettings.RemoveProvider(settings);
    await _settingsManager.SaveAppSettingsAsync(updatedAppSettings);
    _console.MarkupLine($"[bold]{settings.Provider}[/] has been removed.");
    return 0;
  }
}