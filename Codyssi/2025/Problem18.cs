using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;

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
  [InlineData("Problem18.txt", 300, 532770)]
  public void Part2(string inputFile, long remaining, long expected)
  {
    var items = GetInput(inputFile);

    var temp = GetCombinations(items, 0, remaining);
    (temp.Quality * temp.UniqueMaterials).Should().Be(expected);
  }

  Dictionary<(int, long), (long, long)> Cache = [];

  (long Quality, long UniqueMaterials) GetCombinations(List<Item> items, int index, long remaining)
  {
    if (index >= items.Count) return (0, 0);
    var key = (index, remaining);
    if (Cache.TryGetValue(key, out var needle)) return needle;
    // First, compute total quality containing the current item
    var current = items[index];
    long myQuality = 0;
    long myUniqueMaterials = 0;
    if (current.Cost <= remaining)
    {
      // Get the total quality of all remaining items that can be bought after buying the current item
      var subs2 = GetCombinations(items, index + 1, remaining - current.Cost);
      myQuality = subs2.Quality + current.Quality;
      myUniqueMaterials = subs2.UniqueMaterials + current.UniqueMaterials;
    }

    // Then look at total quality that can be obtained without the current item. If it is larger, return that instead.
    var subs = GetCombinations(items, index + 1, remaining);
    if (subs.Quality > myQuality || (subs.Quality == myQuality && subs.UniqueMaterials < myUniqueMaterials))
    {
      myQuality = subs.Quality;
      myUniqueMaterials = subs.UniqueMaterials;
    }
    Cache[key] = (myQuality, myUniqueMaterials);
    return Cache[key];
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
