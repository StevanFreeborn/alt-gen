using System.Diagnostics;

namespace AltGen.API.Common;

class Instrumentation : IDisposable
{
  internal const string ActivitySourceName = "AltGen.API";
  internal const string ActivitySourceVersion = "1.0.0";

  public Instrumentation()
  {
    ActivitySource = new ActivitySource(ActivitySourceName, ActivitySourceVersion);
  }

  public ActivitySource ActivitySource { get; }

  public void Dispose()
  {
    ActivitySource.Dispose();
  }
}