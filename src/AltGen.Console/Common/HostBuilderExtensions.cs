namespace AltGen.Console.Common;

static class HostBuilderExtensions
{
  public static CommandApp<GenerateCommand> BuildApp(this IHostBuilder builder)
  {
    var registrar = new TypeRegistrar(builder);
    var app = new CommandApp<GenerateCommand>(registrar);

    app.Configure(static c =>
    {
      c.SetExceptionHandler(static (ex, resolver) =>
      {
        var console = resolver?.Resolve(typeof(IAnsiConsole)) as IAnsiConsole;
        console?.WriteException(ex, ExceptionFormats.ShortenEverything);
        return -99;
      });

      c.AddBranch("config", static c =>
      {
        c.AddCommand<AddConfigCommand>("add");
        c.AddCommand<RemoveConfigCommand>("remove");
        c.AddCommand<ListConfigCommand>("list");
      });
    });

    return app;
  }
}