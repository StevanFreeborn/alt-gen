namespace AltGen.Console.Common;

static class HostBuilderExtensions
{
  public static CommandApp<GenerateCommand> BuildApp(this IHostBuilder builder)
  {
    var registrar = new TypeRegistrar(builder);
    var app = new CommandApp<GenerateCommand>(registrar);

    app.Configure(static c =>
    {
      c.PropagateExceptions();

      c.AddBranch("config", static c =>
      {
        c.AddCommand<AddConfigCommand>("add");
        c.AddCommand<RemoveConfigCommand>("remove");
      });
    });

    return app;
  }
}