namespace AltGen.Console.Tests.Unit;

public class ListConfigCommandTests : IDisposable
{
  readonly Mock<IAppSettingsManager> _mockSettingsManager = new();
  readonly TestConsole _testConsole = new();
  readonly ListConfigCommand _sut;

  public ListConfigCommandTests()
  {
    _sut = new ListConfigCommand(_testConsole, _mockSettingsManager.Object);
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldOutputNoSettings()
  {
    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(false);

    var result = await _sut.ExecuteAsync(null!);

    result.Should().Be(0);

    _testConsole
      .Output
      .Should()
      .Contain("No settings found.");
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsExist_ItShouldOutputSettings()
  {
    var testSettings = new AppSettings([
      new("provider", "key", false)
    ]);

    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(true);

    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(testSettings);

    var result = await _sut.ExecuteAsync(null!);

    result.Should().Be(0);

    _testConsole
      .Output
      .Should()
      .Contain("provider key");
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsExistAndDefault_ItShouldOutputSettings()
  {
    var testSettings = new AppSettings([
      new("provider", "key", true)
    ]);

    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(true);

    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(testSettings);

    var result = await _sut.ExecuteAsync(null!);

    result.Should().Be(0);

    _testConsole
      .Output
      .Should()
      .Contain("provider key (default)");
  }

  public void Dispose()
  {
    _testConsole.Dispose();
    GC.SuppressFinalize(this);
  }
}