namespace AltGen.Console.Tests.Unit;

public class TypeResolverTests
{
  readonly Mock<IHost> _mockHost = new();

  [Fact]
  public void Constructor_WhenCalledWithNullHost_ItShouldThrowArgumentNullException()
  {
    var act = static () => new TypeResolver(null!);

    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Resolve_WhenCalledWithNullType_ItShouldReturnNull()
  {
    var sut = new TypeResolver(_mockHost.Object);

    var result = sut.Resolve(null);

    result.Should().BeNull();
  }

  [Fact]
  public void Resolve_WhenCalledWithValidType_ItShouldReturnService()
  {
    var sut = new TypeResolver(_mockHost.Object);
    var service = typeof(IService);

    _mockHost
      .Setup(x => x.Services.GetService(service))
      .Returns(new Implementation());

    var result = sut.Resolve(service);

    result.Should().NotBeNull();

    _mockHost.Verify(x => x.Services.GetService(service), Times.Once);
  }

  [Fact]
  public void Dispose_WhenCalled_ItShouldDisposeHost()
  {
    var sut = new TypeResolver(_mockHost.Object);

    sut.Dispose();

    _mockHost.Verify(static x => x.Dispose(), Times.Once);
  }

  interface IService { }

  class Implementation : IService { }
}