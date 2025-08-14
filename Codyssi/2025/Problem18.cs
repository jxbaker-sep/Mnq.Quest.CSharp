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
  [InlineData("Problem18.Sample.txt", 8256)]
  // [InlineData("Problem18.txt", 0)]
  public void Part2(string inputFile, long expected)
  {
    var items = GetInput(inputFile);

    var temp = GetCombinations(items, 30)
      .OrderByDescending(items => items.Sum(it => it.Quality))
      .ThenBy(items => items.Sum(it => it.UniqueMaterials))
      .ToList();
    temp.Select(it => it.Sum(it => it.Quality) * it.Sum(it => it.UniqueMaterials))
      .ToList()
      .First()
      .Should().Be(expected);
  }

  IEnumerable<List<Item>> GetCombinations(List<Item> items, long remaining)
  {
    items = items.Where(it => it.Cost <= remaining).ToList();
    if (items.Count == 0) yield break;
    // First, compute all sets containing the first item
    var subs = GetCombinations(items[1..], remaining - items[0].Cost).ToList();
    if (subs.Count == 0) {
      yield return [items[0]];
    }
    foreach (var sub in subs) yield return [items[0], .. sub];

    // Then return all sets not containing the first item
    subs = GetCombinations(items[1..], remaining).ToList();
    foreach (var sub in subs) yield return sub;
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
