namespace AltGen.Console.Tests.Unit;

public class AddConfigCommandTests : IDisposable
{
  readonly Mock<IFileSystem> _fileSystem = new();
  readonly TestConsole _testConsole = new();
  readonly AddConfigCommand _sut;

  public AddConfigCommandTests()
  {
    _sut = new AddConfigCommand(_testConsole, _fileSystem.Object);
  }


  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldCreateSettings()
  {
    var testSettingsPath = "appsettings.json";
    var expectedAppSettings = new AppSettings([
      new("provider", "key", false)
    ]);
    var expectedJson = JsonSerializer.Serialize(expectedAppSettings, JsonOptions.Default);


    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.File.Exists(It.IsAny<string>()))
      .Returns(false);

    var commandSettings = new AddConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "provider",
      Key = "key",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _fileSystem
      .Verify(
        x => x.File.WriteAllTextAsync(testSettingsPath, expectedJson, default),
        Times.Once
      );
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsExist_ItShouldUpdateSettings()
  {
    var testSettingsPath = "appsettings.json";
    var existingAppSettings = new AppSettings([
      new("existing", "existing", false)
    ]);
    var existingJson = JsonSerializer.Serialize(existingAppSettings, JsonOptions.Default);
    var expectedAppSettings = new AppSettings([
      new("existing", "key", true)
    ]);
    var expectedJson = JsonSerializer.Serialize(expectedAppSettings, JsonOptions.Default);

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.File.Exists(It.IsAny<string>()))
      .Returns(true);

    _fileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync(existingJson);

    var commandSettings = new AddConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "existing",
      Key = "key",
      Default = true,
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _fileSystem
      .Verify(
        x => x.File.WriteAllTextAsync(testSettingsPath, expectedJson, default),
        Times.Once
      );
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsExistButCanNotBeDeserialized_ItShouldThrow()
  {
    var testSettingsPath = "appsettings.json";

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.File.Exists(It.IsAny<string>()))
      .Returns(true);

    _fileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync("null");

    var commandSettings = new AddConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "provider",
      Key = "key",
    };

    var act = async () => await _sut.ExecuteAsync(null!, commandSettings);

    await act.Should().ThrowAsync<ConfigException>();
  }

  public void Dispose()
  {
    _testConsole.Dispose();
    GC.SuppressFinalize(this);
  }
}