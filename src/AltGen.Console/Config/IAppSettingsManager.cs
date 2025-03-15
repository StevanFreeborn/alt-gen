namespace AltGen.Console.Config;

interface IAppSettingsManager
{
  bool AppSettingsExist();
  Task<AppSettings> GetAppSettingsAsync();
  Task SaveAppSettingsAsync(AppSettings appSettings);
}