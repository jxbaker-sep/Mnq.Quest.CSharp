using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest06
{
  [Theory]
  [InlineData("Quest06.Sample.1.txt", "RRB@")]
  [InlineData("Quest06.1.txt", "RRWZFPGMZKBM@")]
  public void Part1(string inputFile, string expected)
  {
    var tree = GetInput(inputFile);

    Paths(tree, "RR").GroupToDictionary(it => it.Length, it => it)
      .Single(it => it.Value.Count == 1)
      .Value[0]
      .Should().Be(expected);
  }

  static IEnumerable<string> Paths(IReadOnlyDictionary<string, List<string>> tree, string current)
  {
    foreach (var item in tree.GetValueOrDefault(current) ?? [])
    {
      if (item == "@")
      {
        yield return $"{current}@";
      }
      else foreach (var sub in Paths(tree, item))
      {
        yield return $"{current}{sub}";
      }
    }
  }


  private static IReadOnlyDictionary<string, List<string>> GetInput(string inputFile)
  {
    Dictionary<string, List<string>> result = [];
    foreach (var line in ECLoader.ReadLines(inputFile))
    {
      var temp = line.Split(":");
      result[temp[0]] = [.. temp[1].Split(',')];
    }
    return result;
  }
}
