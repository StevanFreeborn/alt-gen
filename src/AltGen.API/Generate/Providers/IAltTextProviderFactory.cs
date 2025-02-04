namespace AltGen.API.Generate.Providers;

interface IAltTextProviderFactory
{
  IAltTextProvider Create(string provider);
}