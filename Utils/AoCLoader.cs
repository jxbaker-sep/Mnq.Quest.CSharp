namespace Mng.Quest.CSharp.Utils;

public static class AoCLoader
{
  public static List<string> LoadLines(string item)
  {
    var path = $"/home/jxbaker@net.sep.com/dev/Mng.Quest.Input/{item}.txt";
    return [.. File.ReadAllLines(path)];
  }

  public static string LoadFile(string item)
  {
    var path = $"/home/jxbaker@net.sep.com/dev/Mng.Quest.Input/{item}.txt";
    return File.ReadAllText(path);
  }
}