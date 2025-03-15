using System.Globalization;

namespace AltGen.Console.Common;

static class Providers
{
  const string Gemini = "gemini";

  public static bool IsSupported(string provider)
  {
    return provider.ToLower(CultureInfo.InvariantCulture) switch
    {
      Gemini => true,
      _ => false
    };
  }
}