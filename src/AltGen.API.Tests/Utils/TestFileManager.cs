namespace AltGen.API.Tests.Utils;

public static class TestFileManager
{
  public static byte[] GetFile(string testFileName)
  {
    var filePath = Path.Combine(AppContext.BaseDirectory, "Files", testFileName);
    var file = File.ReadAllBytes(filePath);
    return file;
  }

  public static Task<byte[]> GetFileAsync(string testFileName)
  {
    var filePath = Path.Combine(AppContext.BaseDirectory, "Files", testFileName);
    var file = File.ReadAllBytesAsync(filePath);
    return file;
  }
}