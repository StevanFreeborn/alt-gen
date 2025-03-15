namespace AltGen.Console.Config;

sealed class AddConfigCommand(
  IAnsiConsole console,
  IAppSettingsManager settingsManager
) : AsyncCommand<AddConfigCommand.Settings>
{
  readonly IAnsiConsole _console = console;
  readonly IAppSettingsManager _settingsManager = settingsManager;

  public class Settings : CommandSettings
  {
    [CommandArgument(1, "<provider>")]
    [Description("The provider to configure.")]
    public string Provider { get; init; } = string.Empty;

    [CommandArgument(2, "<key>")]
    [Description("The key for the provider.")]
    public string Key { get; init; } = string.Empty;

    [CommandOption("-d|--default")]
    [Description("Set the provider as the default.")]
    public bool Default { get; init; }

    public override ValidationResult Validate()
    {
      if (Providers.IsSupported(Provider) is false)
      {
        return ValidationResult.Error($"The provider '{Provider}' is not supported.");
      }

      return ValidationResult.Success();
    }
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    if (_settingsManager.AppSettingsExist() is false)
    {
      var appSettings = new AppSettings([
        new(settings.Provider, settings.Key, settings.Default)
      ]);
      await _settingsManager.SaveAppSettingsAsync(appSettings);
      _console.MarkupLine($"[bold]{settings.Provider}[/] has been configured.");
      return 0;
    }

    var existingAppSettings = await _settingsManager.GetAppSettingsAsync();
    var updatedAppSettings = existingAppSettings.AddOrUpdateProvider(settings);
    await _settingsManager.SaveAppSettingsAsync(updatedAppSettings);
    _console.MarkupLine($"[bold]{settings.Provider}[/] has been configured.");
    return 0;
  }
}