using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Utils;
using Mng.Quest.CSharp.Utils;
namespace Mnq.Quest.CSharp.Codyssi;

public class Day05
{
  [Theory]
  [InlineData("Day05.Sample.txt", 226)]
  [InlineData("Day05.txt", 653)]
  public void Part1(string inputFile, long expected)
  {
    var islands = GetRanges(inputFile);

    var mds = islands.Select(it => it.ManhattanDistance(Point.Zero)).ToList();

    (mds.Max() - mds.Min()).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample.txt", 114)]
  [InlineData("Day05.txt", 81)]
  public void Part2(string inputFile, long expected)
  {
    var islands = GetRanges(inputFile);

    var closest = islands.MinBy(island => island.ManhattanDistance(Point.Zero))!;

    islands.Except([closest]).Min(island => island.ManhattanDistance(closest)).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample.txt", 1384)]
  [InlineData("Day05.txt", 6945)]
  public void Part3(string inputFile, long expected)
  {
    var islands = GetRanges(inputFile).ToHashSet();

    var current = Point.Zero;

    long total = 0;
    while(islands.Count > 0)
    {
      var mds = islands.Select(it => (island: it, md: it.ManhattanDistance(current))).GroupToDictionary(it => it.md, it => it.island);
      var min = mds.Keys.Min();
      var closest = mds[min].OrderBy(it => it.X).ThenBy(it => it.Y).First();
      total += min;
      current = closest;
      islands.Remove(current);
    }

    total.Should().Be(expected);
  }



  private static List<Point> GetRanges(string inputFile)
  {
    return P.Format("({}, {})", P.Long, P.Long)
      .Select(it => new Point(it.First, it.Second))
      .ParseMany(CodyssiLoader.ReadLines(inputFile).ToList());
  }
}