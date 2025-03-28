namespace AltGen.Console.Tests.Unit;

public class AppSettingsTests
{
  [Fact]
  public void AddOrUpdateProvider_WhenProviderDoesNotExist_ItShouldAddProvider()
  {
    var providerSettings = new ProviderSettings("provider", "key", true);

    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = providerSettings.Provider,
      Key = providerSettings.Key,
      Default = providerSettings.Default,
    };

    var appSettings = new AppSettings([]);

    var updatedAppSettings = appSettings.AddOrUpdateProvider(commandSettings);

    updatedAppSettings.Should().BeEquivalentTo(new AppSettings([providerSettings]));
  }

  [Fact]
  public void AddOrUpdateProvider_WhenProviderDoesExist_ItShouldUpdateProvider()
  {
    var providerSettings = new ProviderSettings("provider", "key", true);
    var existingProviderSettings = new ProviderSettings("provider", "key", false);

    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = providerSettings.Provider,
      Key = providerSettings.Key,
      Default = providerSettings.Default,
    };

    var appSettings = new AppSettings([existingProviderSettings]);
    var updatedAppSettings = appSettings.AddOrUpdateProvider(commandSettings);

    updatedAppSettings.Should().BeEquivalentTo(new AppSettings([providerSettings]));
  }

  [Fact]
  public void AddOrUpdateProvider_WhenExistingProviderIsSetToDefaultAndNewDefaultGiven_ItShouldUpdateProvidersCorrectly()
  {
    var providerSettings = new ProviderSettings("claude", "key", true);
    var existingProviderSettings = new ProviderSettings("gemini", "key", true);

    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = providerSettings.Provider,
      Key = providerSettings.Key,
      Default = providerSettings.Default,
    };

    var appSettings = new AppSettings([existingProviderSettings]);
    var updatedAppSettings = appSettings.AddOrUpdateProvider(commandSettings);

    updatedAppSettings.Should().BeEquivalentTo(new AppSettings([
      providerSettings,
      existingProviderSettings with { Default = false }
    ]));
  }

  [Fact]
  public void AddOrUpdateProvider_WhenExistingProvider_ItShouldAddNewProvider()
  {
    var providerSettings = new ProviderSettings("claude", "key", false);
    var existingProviderSettings = new ProviderSettings("gemini", "key", false);

    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = providerSettings.Provider,
      Key = providerSettings.Key,
      Default = providerSettings.Default,
    };

    var appSettings = new AppSettings([existingProviderSettings]);
    var updatedAppSettings = appSettings.AddOrUpdateProvider(commandSettings);

    updatedAppSettings.Should().BeEquivalentTo(new AppSettings([
      providerSettings,
      existingProviderSettings,
    ]));
  }

  [Fact]
  public void RemoveProvider_WhenProviderExists_ItShouldRemoveProvider()
  {
    var existingProviderSettings = new ProviderSettings("provider", "key", true);

    var commandSettings = new RemoveConfigCommand.Settings()
    {
      Provider = existingProviderSettings.Provider,
    };

    var appSettings = new AppSettings([existingProviderSettings]);

    var updatedAppSettings = appSettings.RemoveProvider(commandSettings);

    updatedAppSettings.Should().BeEquivalentTo(new AppSettings([]));
  }
}