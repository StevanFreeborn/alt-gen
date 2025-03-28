await Host.CreateDefaultBuilder(args)
  .ConfigureLogging(static l => l.ClearProviders())
  .ConfigureServices(static (_, services) =>
  {
    services.AddSingleton<IFileSystem, FileSystem>();
    services.AddSingleton<IAppSettingsManager, AppSettingsManager>();
    services.AddSingleton(AnsiConsole.Console);
    services.AddHttpClient<IAltGenService, AltGenService>()
      .AddStandardResilienceHandler();
  })
  .BuildApp()
  .RunAsync(args);