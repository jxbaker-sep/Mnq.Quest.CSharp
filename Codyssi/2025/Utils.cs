namespace Mnq.Quest.CSharp.Codyssi;

public static class CodyssiLoader
{
  private const string Base = "/Users/jxbaker/dev/Mnq.Quest.CSharp/Codyssi/2025/data/";

  public static List<string> ReadLines(string inputFile) => [.. File.ReadAllLines(Base + inputFile)];
  public static string ReadAllText(string inputFile) => File.ReadAllText("/Users/jxbaker/dev/Mnq.Quest.CSharp/Codyssi/2025/data/" + inputFile);
}