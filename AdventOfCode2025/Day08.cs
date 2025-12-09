
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;
using System.Diagnostics;

namespace Mng.Quest.CSharp.AdventOfCode2025;

record PointWithSet(Point3 Point, DisjointSet Set);

public class Day08
{
  [Theory]
  [InlineData("Day08.Sample.txt", 10, 40)]
  [InlineData("Day08.txt", 1_000, 102816)]
  public void Part1(string inputFile, long connections, long expected)
  {
    var grid = P.Long.Star(",").End().Select(it => new PointWithSet(new Point3(it[0], it[1], it[2]), new DisjointSet()))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    var pairs = grid.Pairs().ToList();

    pairs = [.. pairs.OrderBy(it => it.First.Point.StraightLineDistance(it.Second.Point))];

    // var pairs = grid.Pairs().OrderBy(it => it.First.Point.StraightLineDistance(it.Second.Point)).ToList();

    for (var i = 0; i < connections; i++)
    {
      var (p1, p2) = pairs[i];
      p1.Set.Union(p2.Set);
    }

    var zed = grid.Select(it => it.Set.Find())
      .GroupToCounts()
      .Select(it => (long)it.Value)
      .OrderByDescending(it => it)
      .Take(3).Product()
    .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day08.Sample.txt", 25272)]
  [InlineData("Day08.txt", 100011612)]
  public void Part2(string inputFile, long expected)
  {
    var sw = new Stopwatch();
    sw.Start();
    var grid = P.Long.Star(",").End().Select(it => new PointWithSet(new Point3(it[0], it[1], it[2]), new DisjointSet()))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));
    var t0 = sw.ElapsedMilliseconds;

    var pairs = grid.Pairs().OrderBy(it => it.First.Point.StraightLineDistance(it.Second.Point)).ToList();

    var t1 = sw.ElapsedMilliseconds;

    long result = -1;
    long distinct = grid.Count;
    for (var i = 0; i < pairs.Count; i++)
    {
      var (p1, p2) = pairs[i];
      if (!p1.Set.SameUnion(p2.Set)) distinct -= 1;
      p1.Set.Union(p2.Set);
      if (distinct == 1)
      {
        result = p1.Point.X * p2.Point.X;
        break;
      }
    }
    var t2 = sw.ElapsedMilliseconds;
    Console.WriteLine($"{t0} {t1} {t2}");
    result.Should().Be(expected);
  }
}