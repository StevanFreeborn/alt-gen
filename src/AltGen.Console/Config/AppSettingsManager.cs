namespace AltGen.Console.Config;

class AppSettingsManager(IFileSystem fileSystem) : IAppSettingsManager
{
  const string SettingsFileName = "appsettings.json";
  readonly IFileSystem _fileSystem = fileSystem;

  public bool AppSettingsExist()
  {
    return _fileSystem.Path.Exists(SettingsFileName);
  }

  public async Task<AppSettings> GetAppSettingsAsync()
  {
    var existingJson = await _fileSystem.File.ReadAllTextAsync(GetSettingsPath());
    var existingAppSettings = JsonSerializer.Deserialize<AppSettings>(existingJson, JsonOptions.Default) ?? new AppSettings([]);
    return existingAppSettings;
  }

  public async Task SaveAppSettingsAsync(AppSettings appSettings)
  {
    var json = JsonSerializer.Serialize(appSettings, JsonOptions.Default);
    await _fileSystem.File.WriteAllTextAsync(GetSettingsPath(), json);
  }

  string GetSettingsPath()
  {
    return _fileSystem.Path.Combine(AppContext.BaseDirectory, SettingsFileName);
  }
}