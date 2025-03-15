namespace AltGen.Console.Tests.Unit;

public class RemoveConfigCommandTests : IDisposable
{
  readonly Mock<IAppSettingsManager> _mockSettingsManager = new();
  readonly TestConsole _testConsole = new();
  readonly RemoveConfigCommand _sut;

  public RemoveConfigCommandTests()
  {
    _sut = new RemoveConfigCommand(_testConsole, _mockSettingsManager.Object);
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldThrow()
  {
    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(false);

    var commandSettings = new RemoveConfigCommand.Settings()
    {
      Provider = "provider",
    };

    var action = async () => await _sut.ExecuteAsync(null!, commandSettings);

    await action.Should().ThrowAsync<ConfigException>();
  }


  [Fact]
  public async Task ExecuteAsync_WhenProviderDoesNotExist_ItShouldDoNothing()
  {
    var testSettings = new AppSettings([]);

    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(true);

    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(testSettings);

    var commandSettings = new RemoveConfigCommand.Settings()
    {
      Provider = "provider",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _mockSettingsManager.Verify(x => x.SaveAppSettingsAsync(testSettings), Times.Once);
  }

  [Fact]
  public async Task ExecuteAsync_WhenProviderExists_ItShouldRemoveProviderFromSettings()
  {
    var testSettings = new AppSettings([
      new("provider", "key", false)
    ]);

    var expectedSettings = new AppSettings([]);

    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(true);

    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(testSettings);

    var commandSettings = new RemoveConfigCommand.Settings()
    {
      Provider = "provider",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _mockSettingsManager.Verify(x => x.SaveAppSettingsAsync(expectedSettings), Times.Once);
  }

  public void Dispose()
  {
    _testConsole.Dispose();
    GC.SuppressFinalize(this);
  }
}