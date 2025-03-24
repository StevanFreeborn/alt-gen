namespace AltGen.Console.Tests.Unit;

public class TypeRegistrarTests
{
  readonly Mock<IHostBuilder> _mockBuilder = new();
  readonly TypeRegistrar _sut;

  public TypeRegistrarTests()
  {
    _sut = new TypeRegistrar(_mockBuilder.Object);
  }

  [Fact]
  public void Register_WhenCalled_ItShouldRegisterService()
  {
    var service = typeof(IService);
    var implementation = typeof(Implementation);

    _sut.Register(service, implementation);

    _mockBuilder.Verify(static x => x.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()), Times.Once);
  }

  [Fact]
  public void RegisterInstance_WhenCalled_ItShouldRegisterService()
  {
    var service = typeof(IService);
    var implementation = new Implementation();

    _sut.RegisterInstance(service, implementation);

    _mockBuilder.Verify(static x => x.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()), Times.Once);
  }

  [Fact]
  public void RegisterLazy_WhenCalled_ItShouldRegisterService()
  {
    var service = typeof(IService);

    static Implementation Func()
    {
      return new Implementation();
    }

    _sut.RegisterLazy(service, Func);

    _mockBuilder.Verify(static x => x.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()), Times.Once);
  }

  [Fact]
  public void RegisterLazy_WhenCalledAndFuncIsNull_ItShouldThrowArgumentNullException()
  {
    var service = typeof(IService);

    var act = () => _sut.RegisterLazy(service, null!);

    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Build_WhenCalled_ItShouldReturnTypeResolver()
  {
    _mockBuilder.Setup(static x => x.Build()).Returns(Mock.Of<IHost>());

    var actual = _sut.Build();

    actual.Should().BeOfType<TypeResolver>();

    _mockBuilder.Verify(static x => x.Build(), Times.Once);
  }

  interface IService { }
  class Implementation : IService { }
}