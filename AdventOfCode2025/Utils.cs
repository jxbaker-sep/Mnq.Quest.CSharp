namespace Mng.Quest.CSharp.AdventOfCode2025;

public static class AdventOfCode2025Loader
{
  private const string Base = "/Users/jxbaker/dev/Mnq.Quest.CSharp/AdventOfCode2025/data/";

  public static List<string> ReadLines(string inputFile) => [.. File.ReadAllLines(Base + inputFile)];
  public static string ReadAllText(string inputFile) => File.ReadAllText(Base + inputFile);
}