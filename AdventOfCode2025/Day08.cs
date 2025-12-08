
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day08
{
  [Theory]
  [InlineData("Day08.Sample.txt", 10, 40)]
  [InlineData("Day08.txt", 1_000, 102816)]
  public void Part1(string inputFile, long connections, long expected)
  {
    var grid = P.Long.Star(",").End().Select(it => new Point3(it[0], it[1], it[2]))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    Dictionary<Point3, DisjointSet> sets = [];

    List<(Point3, Point3)> distances = [];

    foreach (var (p1, index) in grid.WithIndices())
    {
      foreach (var p2 in grid[(index + 1)..])
      {
        distances.Add((p1, p2));
      }
    }

    distances = distances.OrderBy(it => it.Item1.StraightLineDistance(it.Item2)).ToList();

    for (var i = 0; i < connections; i++)
    {
      var (p1, p2) = distances[i];
      var dj1 = sets.GetValueOrDefault(p1, new());
      var dj2 = sets.GetValueOrDefault(p2, new());
      dj1.Union(dj2);
      sets[p1] = dj1;
      sets[p2] = dj1;
    }

    var zed = sets.Values.Select(it => it.Find())
      .GroupToCounts()
      .Select(it => (long)it.Value)
      .OrderByDescending(it => it)
      .ToList();
    zed.Take(3).Product()
    .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day08.Sample.txt", 25272)]
  [InlineData("Day08.txt", 100011612)]
  public void Part2(string inputFile, long expected)
  {
    var grid = P.Long.Star(",").End().Select(it => new Point3(it[0], it[1], it[2]))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    Dictionary<Point3, DisjointSet> sets = [];

    List<(Point3, Point3)> distances = [];

    foreach (var (p1, index) in grid.WithIndices())
    {
      foreach (var p2 in grid[(index + 1)..])
      {
        distances.Add((p1, p2));
      }
    }

    distances = distances.OrderBy(it => it.Item1.StraightLineDistance(it.Item2)).ToList();

    long result = -1;
    long distinct = grid.Count;
    for (var i = 0; i < distances.Count; i++)
    {
      var (p1, p2) = distances[i];
      var dj1 = sets.GetValueOrDefault(p1, new());
      var dj2 = sets.GetValueOrDefault(p2, new());
      if (!dj1.SameUnion(dj2)) distinct -= 1;
      dj1.Union(dj2);
      sets[p1] = dj1;
      sets[p2] = dj1;
      if (distinct == 1)
      {
        result = p1.X * p2.X;
        break;
      }
    }

    result.Should().Be(expected);
  }
}