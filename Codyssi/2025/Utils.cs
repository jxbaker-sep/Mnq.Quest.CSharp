namespace Mnq.Quest.CSharp.Codyssi;

public static class CodyssiLoader
{
  private const string Base = "/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/Codyssi/2025/data/";

  public static List<string> ReadLines(string inputFile) => [.. File.ReadAllLines(Base + inputFile)];
  public static string ReadAllText(string inputFile) => File.ReadAllText("/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/Codyssi/2025/data/" + inputFile);
}