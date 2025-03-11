namespace AltGen.Console.Tests.Unit;

public class RemoveConfigCommandTests : IDisposable
{
  readonly Mock<IFileSystem> _fileSystem = new();
  readonly TestConsole _testConsole = new();
  readonly RemoveConfigCommand _sut;

  public RemoveConfigCommandTests()
  {
    _sut = new RemoveConfigCommand(_fileSystem.Object, _testConsole);
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldThrow()
  {
    var testSettingsPath = "appsettings.json";

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.Path.Exists(It.IsAny<string>()))
      .Returns(false);

    var commandSettings = new RemoveConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "provider",
    };

    var action = async () => await _sut.ExecuteAsync(null!, commandSettings);

    await action.Should().ThrowAsync<ConfigException>();
  }


  [Fact]
  public async Task ExecuteAsync_WhenProviderDoesNotExist_ItShouldDoNothing()
  {
    var testSettingsPath = "appsettings.json";
    var testSettings = new AppSettings([]);
    var testSettingsJson = JsonSerializer.Serialize(testSettings, JsonOptions.Default);

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.Path.Exists(It.IsAny<string>()))
      .Returns(true);

    _fileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync(testSettingsJson);

    var commandSettings = new RemoveConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "provider",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _fileSystem
      .Verify(
        x => x.File.WriteAllTextAsync(testSettingsPath, testSettingsJson, default),
        Times.Once
      );
  }

  [Fact]
  public async Task ExecuteAsync_WhenProviderExists_ItShouldRemoveProviderFromSettings()
  {
    var testSettingsPath = "appsettings.json";
    var testSettings = new AppSettings([
      new("provider", "key", false)
    ]);
    var testSettingsJson = JsonSerializer.Serialize(testSettings, JsonOptions.Default);

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.Path.Exists(It.IsAny<string>()))
      .Returns(true);

    _fileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync(testSettingsJson);

    var commandSettings = new RemoveConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "provider",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    var expectedSettings = new AppSettings([]);
    var expectedSettingsJson = JsonSerializer.Serialize(expectedSettings, JsonOptions.Default);

    _fileSystem
      .Verify(
        x => x.File.WriteAllTextAsync(testSettingsPath, expectedSettingsJson, default),
        Times.Once
      );
  }

  public void Dispose()
  {
    _testConsole.Dispose();
    GC.SuppressFinalize(this);
  }
}