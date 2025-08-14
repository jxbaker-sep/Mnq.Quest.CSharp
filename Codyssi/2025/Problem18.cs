using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using System.Net;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem18
{
  [Theory]
  [InlineData("Problem18.Sample.txt", 90)]
  [InlineData("Problem18.txt", 66)]
  public void Part1(string inputFile, long expected)
  {
    var items = GetInput(inputFile);

    items.OrderByDescending(it => it.Quality)
      .ThenByDescending(it => it.Cost)
      .Take(5)
      .Sum(it => it.UniqueMaterials)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem18.Sample.txt", 30, 8256)]
  [InlineData("Problem18.Sample.txt", 150, 59388)]
  [InlineData("Problem18.txt", 30, 57838)]
  [InlineData("Problem18.txt", 300, 0)]
  public void Part2(string inputFile, long remaining, long expected)
  {
    var items = GetInput(inputFile);

    var temp = GetCombinations(items, 0, remaining)
      .OrderByDescending(items => items.Sum(it => it.Quality))
      .ThenBy(items => items.Sum(it => it.UniqueMaterials))
      .Select(it => it.Sum(it => it.Quality) * it.Sum(it => it.UniqueMaterials))
      .First()
      .Should().Be(expected);
  }

  Dictionary<(int, long), List<List<Item>>> Cache = [];

  List<List<Item>> GetCombinations(List<Item> items, int index, long remaining)
  {
    if (index >= items.Count) return [];
    var key = (index, remaining);
    if (Cache.TryGetValue(key, out var needle)) return needle;
    // First, compute all sets containing the first item
    var current = items[index];
    List<List<Item>> result = [];
    if (current.Cost <= remaining)
    {
      var subs2 = GetCombinations(items, index + 1, remaining - current.Cost);
      if (subs2.Count == 0)
      {
        result.Add([current]);
      }
      foreach (var sub in subs2) result.Add([current, .. sub]);
    }

    // Then return all sets not containing the first item that couldn't contain the first item
    var subs = GetCombinations(items, index + 1, remaining);
    foreach (var sub in subs.Where(it => remaining - it.Sum(it => it.Cost) < current.Cost)) result.Add(sub);
    Cache[key] = result;
    return result;
  }

  record Item(long Index, string Code, long Quality, long Cost, long UniqueMaterials);

  private static List<Item> GetInput(string inputFile)
  {
    return P.Format("{} {} | Quality : {}, Cost : {}, Unique Materials : {}",
      P.Long, P.Word, P.Long, P.Long, P.Long)
      .Select(it => new Item(it.First, it.Second, it.Third, it.Fourth, it.Fifth))
      .ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}
