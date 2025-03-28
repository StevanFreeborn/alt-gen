namespace AltGen.Console.Tests.Unit;

public class AddConfigCommandTests : IDisposable
{
  readonly Mock<IAppSettingsManager> _mockSettingsManager = new();
  readonly TestConsole _testConsole = new();
  readonly AddConfigCommand _sut;

  public AddConfigCommandTests()
  {
    _sut = new AddConfigCommand(_testConsole, _mockSettingsManager.Object);
  }

  [Fact]
  public void Validate_WhenProviderIsNotSupported_ItShouldReturnError()
  {
    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = "unsupported",
      Key = "key",
    };

    var result = commandSettings.Validate();

    result.Successful.Should().BeFalse();
  }

  [Fact]
  public void Validate_WhenProviderIsSupported_ItShouldReturnSuccess()
  {
    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = "gemini",
      Key = "key",
    };

    var result = commandSettings.Validate();

    result.Successful.Should().BeTrue();
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldCreateSettings()
  {
    var expectedAppSettings = new AppSettings([
      new("provider", "key", false)
    ]);

    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(false);

    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = "provider",
      Key = "key",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _mockSettingsManager.Verify(
      static x => x.SaveAppSettingsAsync(It.Is<AppSettings>(
        static x => x.Providers[0].Provider == "provider" &&
          x.Providers[0].Key == "key" &&
          x.Providers[0].Default == false
      )),
      Times.Once
    );
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsExist_ItShouldUpdateSettings()
  {
    var existingAppSettings = new AppSettings([
      new("existing", "existing", false)
    ]);

    var expectedAppSettings = new AppSettings([
      new("existing", "key", true)
    ]);

    _mockSettingsManager
      .Setup(static x => x.AppSettingsExist())
      .Returns(true);

    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(existingAppSettings);

    var commandSettings = new AddConfigCommand.Settings()
    {
      Provider = "existing",
      Key = "key",
      Default = true,
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _mockSettingsManager.Verify(
      static x => x.SaveAppSettingsAsync(It.Is<AppSettings>(
        static x => x.Providers[0].Provider == "existing" &&
          x.Providers[0].Key == "key" &&
          x.Providers[0].Default == true
      )),
      Times.Once
    );
  }

  public void Dispose()
  {
    _testConsole.Dispose();
    GC.SuppressFinalize(this);
  }
}