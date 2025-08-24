namespace Mnq.Quest.CSharp.EverybodyCodes;

public static class ECLoader
{
  private const string Base = "/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/EverybodyCodes/Q2024/data/";

  public static List<string> ReadLines(string inputFile) => [.. File.ReadAllLines(Base + inputFile)];
  public static string ReadAllText(string inputFile) => File.ReadAllText(Base + inputFile);
}