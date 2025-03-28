namespace AltGen.Console.Common;

sealed class TypeResolver(IHost host) : ITypeResolver, IDisposable
{
  readonly IHost _host = host ?? throw new ArgumentNullException(nameof(host));

  public object? Resolve(Type? type)
  {
    if (type is null)
    {
      return null;
    }

    return _host.Services.GetService(type);
  }

  public void Dispose()
  {
    if (_host is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }
}