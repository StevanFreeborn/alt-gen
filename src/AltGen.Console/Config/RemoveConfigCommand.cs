namespace AltGen.Console.Config;

sealed class RemoveConfigCommand() : AsyncCommand<RemoveConfigCommand.Settings>
{
  public class Settings : CommandSettings
  {
    [CommandArgument(1, "<provider>")]
    [Description("The provider to remove.")]
    public string Provider { get; init; } = string.Empty;
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    return 0;
  }
}