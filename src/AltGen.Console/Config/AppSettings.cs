namespace AltGen.Console.Config;

record AppSettings
{
  public const string SettingsFileName = "appsettings.json";

  public ProviderSettings[] Providers { get; init; } = [];

  public AppSettings(ProviderSettings[] providers)
  {
    Providers = providers;
  }

  public AppSettings AddOrUpdateProvider(AddConfigCommand.Settings settings)
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

  public AppSettings RemoveProvider(RemoveConfigCommand.Settings settings)
  {
    var updatedProviders = Providers.Where(p => p.Provider != settings.Provider);
    return this with { Providers = [.. updatedProviders] };
  }

  public ProviderSettings[] GetProviders()
  {
    return Providers;
  }

  public ProviderSettings? GetProvider(string provider)
  {
    return Providers.FirstOrDefault(p => p.Provider == provider);
  }

  public ProviderSettings? GetDefaultProvider()
  {
    return Providers.FirstOrDefault(static p => p.Default);
  }
}