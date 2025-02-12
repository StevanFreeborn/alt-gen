// TODO: Need to implement config
// command. This should allow
// the user to set default values
// for provider keys and a default
// provider.
//
// For example setting a providers key
// i.e. altgen config gemini mykey
//
// Or setting provider as default
// i.e. altgen config gemini mykey --default

await Host.CreateDefaultBuilder(args)
  .ConfigureLogging(static l => l.ClearProviders())
  .ConfigureServices(static (_, services) =>
  {
    services.AddSingleton<IFileSystem, FileSystem>();
    services.AddSingleton(AnsiConsole.Console);
    services.AddHttpClient<IAltGenService, AltGenService>()
      .AddStandardResilienceHandler();
  })
  .BuildApp()
  .RunAsync(args);
