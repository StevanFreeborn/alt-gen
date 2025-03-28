namespace AltGen.Console.Tests.Unit;

public class AppSettingsManagerTests
{
  readonly Mock<IFileSystem> _mockFileSystem = new();
  readonly AppSettingsManager _sut;

  public AppSettingsManagerTests()
  {
    _mockFileSystem
      .Setup(static x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
      .Returns("path");

    _sut = new AppSettingsManager(_mockFileSystem.Object);
  }

  [Fact]
  public void AppSettingsExist_WhenSettingsDoNotExist_ItShouldReturnFalse()
  {
    _mockFileSystem
      .Setup(static x => x.Path.Exists(It.IsAny<string>()))
      .Returns(false);

    var result = _sut.AppSettingsExist();

    result.Should().BeFalse();
  }

  [Fact]
  public void AppSettingsExist_WhenSettingsExist_ItShouldReturnTrue()
  {
    _mockFileSystem
      .Setup(static x => x.Path.Exists(It.IsAny<string>()))
      .Returns(true);

    var result = _sut.AppSettingsExist();

    result.Should().BeTrue();
  }

  [Fact]
  public async Task GetAppSettingsAsync_WhenSettingsExist_ItShouldReturnSettings()
  {
    var testSettings = new AppSettings([
      new("provider", "key", false)
    ]);

    var json = JsonSerializer.Serialize(testSettings, JsonOptions.Default);

    _mockFileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync(json);

    var result = await _sut.GetAppSettingsAsync();

    result.Should().BeEquivalentTo(testSettings);
  }

  [Fact]
  public async Task GetAppSettingsAsync_WhenDeserializingSettingsIsNull_ItShouldReturnEmptySettings()
  {
    _mockFileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ReturnsAsync("null");

    var result = await _sut.GetAppSettingsAsync();

    result.Should().BeEquivalentTo(new AppSettings([]));
  }

  [Fact]
  public async Task GetAppSettingsAsync_WhenSettingsDoNotExist_ItShouldThrow()
  {
    _mockFileSystem
      .Setup(static x => x.File.ReadAllTextAsync(It.IsAny<string>(), default))
      .ThrowsAsync(new FileNotFoundException());

    var action = _sut.GetAppSettingsAsync;

    await action.Should().ThrowAsync<FileNotFoundException>();
  }

  [Fact]
  public async Task SaveAppSettingsAsync_WhenCalled_ItShouldSaveSettings()
  {
    _mockFileSystem
      .Setup(static x => x.File.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), default))
      .Returns(Task.CompletedTask);

    var testSettings = new AppSettings([
      new("provider", "key", false)
    ]);

    var json = JsonSerializer.Serialize(testSettings, JsonOptions.Default);

    await _sut.SaveAppSettingsAsync(testSettings);

    _mockFileSystem.Verify(x => x.File.WriteAllTextAsync(It.IsAny<string>(), json, default), Times.Once);
  }
}