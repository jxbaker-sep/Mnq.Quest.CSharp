
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

record PSet(Point3 Point, DisjointSet Set);

public class Day08
{
  [Theory]
  [InlineData("Day08.Sample.txt", 10, 40)]
  [InlineData("Day08.txt", 1_000, 102816)]
  public void Part1(string inputFile, long connections, long expected)
  {
    var grid = P.Long.Star(",").End().Select(it => new PSet(new Point3(it[0], it[1], it[2]), new DisjointSet()))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    List<(PSet, PSet)> distances = [];

    foreach (var (p1, index) in grid.WithIndices())
    {
      foreach (var p2 in grid[(index + 1)..])
      {
        distances.Add((p1, p2));
      }
    }

    distances = distances.OrderBy(it => it.Item1.Point.StraightLineDistance(it.Item2.Point)).ToList();

    for (var i = 0; i < connections; i++)
    {
      var (p1, p2) = distances[i];
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
    var grid = P.Long.Star(",").End().Select(it => new PSet(new Point3(it[0], it[1], it[2]), new DisjointSet()))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    List<(PSet, PSet)> distances = [];

    foreach (var (p1, index) in grid.WithIndices())
    {
      foreach (var p2 in grid[(index + 1)..])
      {
        distances.Add((p1, p2));
      }
    }

    distances = distances.OrderBy(it => it.Item1.Point.StraightLineDistance(it.Item2.Point)).ToList();

    long result = -1;
    long distinct = grid.Count;
    for (var i = 0; i < distances.Count; i++)
    {
      var (p1, p2) = distances[i];
      if (!p1.Set.SameUnion(p2.Set)) distinct -= 1;
      p1.Set.Union(p2.Set);
      if (distinct == 1)
      {
        result = p1.Point.X * p2.Point.X;
        break;
      }
    }

    result.Should().Be(expected);
  }
}