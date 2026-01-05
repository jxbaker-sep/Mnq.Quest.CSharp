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

    Paths(tree, "RR", [], false).GroupToDictionary(it => it.Length, it => it)
      .Single(it => it.Value.Count == 1)
      .Value[0]
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest06.2.txt", "RJVDGLRZJF@")]
  public void Part2(string inputFile, string expected)
  {
    var tree = GetInput(inputFile);

    Paths(tree, "RR", [], true).GroupToDictionary(it => it.Length, it => it)
      .Single(it => it.Value.Count == 1)
      .Value[0]
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest06.3.txt", "RSHPKNPBKQCS@")]
  public void Part3(string inputFile, string expected)
  {
    var tree = GetInput(inputFile);

    Paths(tree, "RR", [], true).GroupToDictionary(it => it.Length, it => it)
      .Single(it => it.Value.Count == 1)
      .Value[0]
      .Should().Be(expected);
  }

  static IEnumerable<string> Paths(IReadOnlyDictionary<string, List<string>> tree, string current, HashSet<string> previous, bool firstLetterOnly)
  {
    if (previous.Contains(current)) yield break;
    HashSet<string> currentPath = [..previous, current];
    var prefix = firstLetterOnly ? current[..1] : current;
    foreach (var item in tree.GetValueOrDefault(current) ?? [])
    {
      if (item == "@")
      {
        yield return $"{prefix}@";
      }
      else foreach (var sub in Paths(tree, item, currentPath, firstLetterOnly))
      {
        yield return $"{prefix}{sub}";
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
