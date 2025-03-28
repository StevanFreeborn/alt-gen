namespace AltGen.API.Generate.Providers;

class AltTextProviderFactory(IServiceProvider serviceProvider) : IAltTextProviderFactory
{
  readonly IServiceProvider _serviceProvider = serviceProvider;

  public IAltTextProvider Create(string provider)
  {
    return provider.ToLowerInvariant() switch
    {
      LLMProvider.Gemini => _serviceProvider.GetRequiredKeyedService<IAltTextProvider>(LLMProvider.Gemini),
      _ => throw new NotSupportedException($"The provider '{provider}' is not supported.")
    };
  }
}