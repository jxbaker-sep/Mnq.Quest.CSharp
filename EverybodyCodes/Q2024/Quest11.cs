using FluentAssertions;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest11
{
  [Theory]
  [InlineData("Quest11.1.Sample.txt", 8)]
  [InlineData("Quest11.1.txt", 48)]
  public void Part1(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    Find(grid, "A", 4).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest11.2.txt", 196466)]
  public void Part2(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    Find(grid, "Z", 10).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest11.3.Sample.txt", 268815)]
  [InlineData("Quest11.3.txt", 1043058809124)]
  public void Part3(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    var results = grid.Keys.Select(it => Find(grid, it, 20)).ToList();

    (results.Max() - results.Min()).Should().Be(expected);
  }

  private readonly Dictionary<(string, long), long> Cache = [];
  private long Find(Dictionary<string, List<string>> grid, string node, long generations)
  {
    if (generations == 0) return 1;
    var key = (node, generations);
    if (Cache.TryGetValue(key, out var found)) return found;
    var result = grid[node].Sum(it => Find(grid, it, generations - 1));
    Cache[key] = result;
    return result;
  }

  private static Dictionary<string, List<string>> GetInput(string inputFile)
  {
    Dictionary<string, List<string>> result = [];
    foreach (var line in ECLoader.ReadLines(inputFile))
    {
      var a = line.Split(':');
      var b = a[1].Split(',').ToList();
      result[a[0]] = b;
    }

    return result;
  }

}
