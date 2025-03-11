using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace AltGen.Console.Tests.Unit;

public class AddConfigCommandTests
{
  readonly Mock<IFileSystem> _fileSystem = new();
  readonly IAnsiConsole _testConsole = new TestConsole();
  readonly AddConfigCommand _sut;

  public AddConfigCommandTests()
  {
    _sut = new AddConfigCommand(_testConsole, _fileSystem.Object);
  }


  [Fact]
  public async Task ExecuteAsync_WhenSettingsDoNotExist_ItShouldCreateSettings()
  {
    var testSettingsPath = "appsettings.json";

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
        x => x.File.WriteAllTextAsync(testSettingsPath, It.IsAny<string>(), default),
        Times.Once
      );
  }

  [Fact]
  public async Task ExecuteAsync_WhenSettingsExist_ItShouldUpdateSettings()
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
      .ReturnsAsync("{}");

    var commandSettings = new AddConfigCommand.Settings(_fileSystem.Object)
    {
      Provider = "provider",
      Key = "key",
    };

    var result = await _sut.ExecuteAsync(null!, commandSettings);

    result.Should().Be(0);

    _fileSystem
      .Verify(
        x => x.File.WriteAllTextAsync(testSettingsPath, It.IsAny<string>(), default),
        Times.Once
      );
  }
}