namespace AltGen.API.Common;

static class WebAppBuilderExtensions
{
  public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
  {
    var seq = new SeqOptions();
    builder.Configuration.GetSection(nameof(SeqOptions)).Bind(seq);

    if (seq.IsEnabled is false)
    {
      return builder;
    }

    builder.Services.AddSingleton<Instrumentation>();

    builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource =>
      {
        resource.AddService(Instrumentation.ActivitySourceName, Instrumentation.ActivitySourceVersion);
        resource.AddAttributes(new Dictionary<string, object>
        {
          ["service.name"] = Instrumentation.ActivitySourceName,
          ["service.version"] = Instrumentation.ActivitySourceVersion,
          ["service.instance.id"] = Environment.MachineName,
          ["service.namespace"] = "stevesbot",
          ["service.environment"] = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "production",
        });
      })
      .WithLogging(lb => lb.AddOtlpExporter(o =>
      {
        o.Protocol = OtlpExportProtocol.HttpProtobuf;
        o.Endpoint = new Uri(seq.LogEndpoint);
        o.Headers = seq.AuthHeader;
      }))
      .WithTracing(tb =>
      {
        tb.AddSource(Instrumentation.ActivitySourceName);
        tb.AddAspNetCoreInstrumentation();
        tb.AddHttpClientInstrumentation();
        tb.AddOtlpExporter(o =>
        {
          o.Protocol = OtlpExportProtocol.HttpProtobuf;
          o.Endpoint = new Uri(seq.TraceEndpoint);
          o.Headers = seq.AuthHeader;
        });
      });

    return builder;
  }
}