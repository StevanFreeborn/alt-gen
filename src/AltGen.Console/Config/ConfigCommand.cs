
using Microsoft.Extensions.Configuration;

namespace AltGen.Console.Config;

// TODO: We should refactor this so that
// we have a config add and a config remove command
// the config add will have basically the same code
// as below but the config remove will just
// accept a provider identifier and remove it

sealed class ConfigCommand(
  IAnsiConsole console,
  IFileSystem fileSystem,
  IConfiguration config
) : AsyncCommand<ConfigCommand.Settings>
{
  static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
  };
  readonly IAnsiConsole _console = console;
  readonly IFileSystem _fileSystem = fileSystem;
  readonly IConfiguration _config = config;

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
    var updatedAppSettings = existingAppSettings.Update(settings);
    var udpatedJson = JsonSerializer.Serialize(updatedAppSettings, JsonOptions);
    await _fileSystem.File.WriteAllTextAsync(settings.SettingsPath, udpatedJson);
    _console.MarkupLine($"[bold]{settings.Provider}[/] has been configured.");
    return 0;
  }
}

record AppSettings(ProviderSettings[] Providers)
{
  public AppSettings Update(ConfigCommand.Settings settings)
  {
    var updatedProviders = Providers.Select(p =>
    {
      if (p.Provider == settings.Provider)
      {
        return p with { Key = settings.Key, Default = settings.Default };
      }

      if (p.Provider != settings.Provider && settings.Default)
      {
        return p with { Default = false };
      }

      return p;
    });

    var provider = Providers.FirstOrDefault(p => p.Provider == settings.Provider);

    if (provider is null)
    {
      var newProvider = new ProviderSettings(settings.Provider, settings.Key, settings.Default);
      return this with { Providers = [.. updatedProviders, newProvider] };
    }

    return this with { Providers = [.. updatedProviders] };
  }
}

record ProviderSettings(string Provider, string Key, bool Default);

class ConfigException(string message) : Exception(message)
{
}