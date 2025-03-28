using System.Collections;

namespace AltGen.Console.Tests.Unit;

public class GenerateCommandTests : IDisposable
{
  readonly TestConsole _testConsole = new();
  readonly Mock<IFileSystem> _mockFileSystem = new();
  readonly Mock<IAltGenService> _mockAltGenService = new();
  readonly Mock<IAppSettingsManager> _mockSettingsManager = new();
  readonly GenerateCommand _sut;

  public GenerateCommandTests()
  {
    _sut = new GenerateCommand(
      _testConsole,
      _mockFileSystem.Object,
      _mockAltGenService.Object,
      _mockSettingsManager.Object
    );
  }

  [Fact]
  public void Validate_WhenPathDoesNotExist_ItShouldReturnError()
  {
    _mockFileSystem
        .Setup(static x => x.File.Exists(It.IsAny<string>()))
        .Returns(false);

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Path = "C:/path/to/image.jpg"
    };

    var result = settings.Validate();

    result.Successful.Should().BeFalse();
  }

  [Fact]
  public void Validate_WhenPathIsNotAnImage_ItShouldReturnError()
  {
    _mockFileSystem
        .Setup(static x => x.File.Exists(It.IsAny<string>()))
        .Returns(true);

    _mockFileSystem
      .Setup(static x => x.Path.GetExtension(It.IsAny<string>()))
      .Returns(".txt");

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Path = "C:/path/to/image.txt"
    };

    var result = settings.Validate();

    result.Successful.Should().BeFalse();
  }

  [Fact]
  public void Validate_WhenPathIsValidImage_ItShouldReturnSuccess()
  {
    _mockFileSystem
        .Setup(static x => x.File.Exists(It.IsAny<string>()))
        .Returns(true);

    _mockFileSystem
        .Setup(static x => x.Path.GetExtension(It.IsAny<string>()))
        .Returns(".jpg");

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Path = "C:/path/to/image.jpg"
    };

    var result = settings.Validate();

    result.Successful.Should().BeTrue();
  }

  [Fact]
  public async Task ExecuteAsync_WhenProviderIsNotProvided_ItShouldThrowException()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([]));

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object);

    var action = async () => await _sut.ExecuteAsync(null!, settings);

    await action.Should().ThrowAsync<AltTextException>();
  }

  [Fact]
  public async Task ExecuteAsync_WhenProviderIsProvidedOnCommandLine_ItShouldUseThatProvider()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([]));

    _mockFileSystem
      .Setup(static x => x.Path.GetFileName(It.IsAny<string>()))
      .Returns("image.jpg");

    _mockFileSystem
      .Setup(static x => x.File.ReadAllBytesAsync(It.IsAny<string>(), default))
      .ReturnsAsync([0x00]);

    _mockFileSystem
      .Setup(static x => x.Path.GetExtension(It.IsAny<string>()))
      .Returns(".jpg");

    _mockAltGenService
      .Setup(static x => x.GenerateAltTextAsync(It.IsAny<GenerateAltTextRequest>()))
      .ReturnsAsync("alt text");

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Provider = "gemini",
      Key = "key",
      Path = "C:/path/to/image.jpg"
    };

    var result = await _sut.ExecuteAsync(null!, settings);

    result.Should().Be(0);

    _mockAltGenService.Verify(
      static x => x.GenerateAltTextAsync(It.Is<GenerateAltTextRequest>(
        static x => x.Provider == "gemini" &&
          x.ProviderKey == "key" &&
          x.FileName == "image.jpg" &&
          x.Image.Length == 1 &&
          x.ContentType == "image/jpeg"
      )),
      Times.Once
    );
  }

  [Fact]
  public async Task ExecuteAsync_WhenProviderIsNotProvidedOnCommandLine_ItShouldUseDefaultProvider()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([
        new("gemini", "key", true)
      ]));

    _mockFileSystem
      .Setup(static x => x.Path.GetFileName(It.IsAny<string>()))
      .Returns("image.jpg");

    _mockFileSystem
      .Setup(static x => x.File.ReadAllBytesAsync(It.IsAny<string>(), default))
      .ReturnsAsync([0x00]);

    _mockFileSystem
      .Setup(static x => x.Path.GetExtension(It.IsAny<string>()))
      .Returns(".jpg");

    _mockAltGenService
      .Setup(static x => x.GenerateAltTextAsync(It.IsAny<GenerateAltTextRequest>()))
      .ReturnsAsync("alt text");

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Key = "key",
      Path = "C:/path/to/image.jpg"
    };

    var result = await _sut.ExecuteAsync(null!, settings);

    result.Should().Be(0);

    _mockAltGenService.Verify(
      static x => x.GenerateAltTextAsync(It.Is<GenerateAltTextRequest>(
        static x => x.Provider == "gemini" &&
          x.ProviderKey == "key" &&
          x.FileName == "image.jpg" &&
          x.Image.Length == 1 &&
          x.ContentType == "image/jpeg"
      )),
      Times.Once
    );
  }

  [Fact]
  public async Task ExecuteAsync_WhenProviderIsNotSupported_ItShouldThrowException()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([]));

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Provider = "unsupported",
      Key = "key",
      Path = "C:/path/to/image.jpg"
    };

    var action = async () => await _sut.ExecuteAsync(null!, settings);

    await action.Should().ThrowAsync<AltTextException>();
  }

  [Fact]
  public async Task ExecuteAsync_WhenNoKeyIsProvided_ItShouldThrowException()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([]));

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Provider = "gemini",
      Path = "C:/path/to/image.jpg"
    };

    var action = async () => await _sut.ExecuteAsync(null!, settings);

    await action.Should().ThrowAsync<AltTextException>();
  }

  [Fact]
  public async Task ExecuteAsync_WhenKeyIsProvidedOnCommandLine_ItShouldUseThatKey()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([]));

    _mockFileSystem
      .Setup(static x => x.Path.GetFileName(It.IsAny<string>()))
      .Returns("image.jpg");

    _mockFileSystem
      .Setup(static x => x.File.ReadAllBytesAsync(It.IsAny<string>(), default))
      .ReturnsAsync([0x00]);

    _mockFileSystem
      .Setup(static x => x.Path.GetExtension(It.IsAny<string>()))
      .Returns(".jpg");

    _mockAltGenService
      .Setup(static x => x.GenerateAltTextAsync(It.IsAny<GenerateAltTextRequest>()))
      .ReturnsAsync("alt text");

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Provider = "gemini",
      Key = "key",
      Path = "C:/path/to/image.jpg"
    };

    var result = await _sut.ExecuteAsync(null!, settings);

    result.Should().Be(0);

    _mockAltGenService.Verify(
      static x => x.GenerateAltTextAsync(It.Is<GenerateAltTextRequest>(
        static x => x.Provider == "gemini" &&
          x.ProviderKey == "key" &&
          x.FileName == "image.jpg" &&
          x.Image.Length == 1 &&
          x.ContentType == "image/jpeg"
      )),
      Times.Once
    );
  }

  [Fact]
  public async Task ExecuteAsync_WhenNoKeyIsProvidedOnCommandLine_ItShouldUseDefaultKey()
  {
    _mockSettingsManager
      .Setup(static x => x.GetAppSettingsAsync())
      .ReturnsAsync(new AppSettings([
        new("gemini", "key", true)
      ]));

    _mockFileSystem
      .Setup(static x => x.Path.GetFileName(It.IsAny<string>()))
      .Returns("image.jpg");

    _mockFileSystem
      .Setup(static x => x.File.ReadAllBytesAsync(It.IsAny<string>(), default))
      .ReturnsAsync([0x00]);

    _mockFileSystem
      .Setup(static x => x.Path.GetExtension(It.IsAny<string>()))
      .Returns(".jpg");

    _mockAltGenService
      .Setup(static x => x.GenerateAltTextAsync(It.IsAny<GenerateAltTextRequest>()))
      .ReturnsAsync("alt text");

    var settings = new GenerateCommand.Settings(_mockFileSystem.Object)
    {
      Provider = "gemini",
      Path = "C:/path/to/image.jpg"
    };

    var result = await _sut.ExecuteAsync(null!, settings);

    result.Should().Be(0);

    _mockAltGenService.Verify(
      static x => x.GenerateAltTextAsync(It.Is<GenerateAltTextRequest>(
        static x => x.Provider == "gemini" &&
          x.ProviderKey == "key" &&
          x.FileName == "image.jpg" &&
          x.Image.Length == 1 &&
          x.ContentType == "image/jpeg"
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