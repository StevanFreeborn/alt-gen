namespace AltGen.Console.Common;

static class HostBuilderExtensions
{
  public static CommandApp<GenerateCommand> BuildApp(this IHostBuilder builder)
  {
    var registrar = new TypeRegistrar(builder);
    var app = new CommandApp<GenerateCommand>(registrar);
    return app;
  }
}