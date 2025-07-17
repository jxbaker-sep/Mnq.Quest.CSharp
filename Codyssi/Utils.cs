namespace Mnq.Quest.CSharp.Codyssi;

public static class CodyssiLoader
{
  public static string[] ReadLines(string inputFile) => File.ReadAllLines("/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/Codyssi/data/" + inputFile);
}