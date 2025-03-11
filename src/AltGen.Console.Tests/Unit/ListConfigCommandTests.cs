namespace AltGen.Console.Tests.Unit;

public class ListConfigCommandTests : IDisposable
{
  readonly Mock<IFileSystem> _fileSystem = new();
  readonly TestConsole _testConsole = new();
  readonly ListConfigCommand _sut;

  public ListConfigCommandTests()
  {
    _sut = new ListConfigCommand(_testConsole, _fileSystem.Object);
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldOutputNoSettings()
  {
    var testSettingsPath = "appsettings.json";

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.File.Exists(It.IsAny<string>()))
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
    var testSettingsPath = "appsettings.json";
    var testSettings = new AppSettings([
      new("provider", "key", false)
    ]);
    var testSettingsJson = JsonSerializer.Serialize(testSettings, JsonOptions.Default);

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.File.Exists(It.IsAny<string>()))
      .Returns(true);

    _fileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync(testSettingsJson);

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
    var testSettingsPath = "appsettings.json";
    var testSettings = new AppSettings([
      new("provider", "key", true)
    ]);
    var testSettingsJson = JsonSerializer.Serialize(testSettings, JsonOptions.Default);

    _fileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(testSettingsPath);

    _fileSystem
      .Setup(static x => x.File.Exists(It.IsAny<string>()))
      .Returns(true);

    _fileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync(testSettingsJson);

    var result = await _sut.ExecuteAsync(null!);

    result.Should().Be(0);

    _testConsole
      .Output
      .Should()
      .Contain("provider key (default)");
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsCanNotBeDeserialized_ItShouldThrowConfigException()
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

    var action = async () => await _sut.ExecuteAsync(null!);

    await action.Should().ThrowAsync<ConfigException>();
  }

  public void Dispose()
  {
    _testConsole.Dispose();
    GC.SuppressFinalize(this);
  }
}