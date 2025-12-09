
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;
using System.Diagnostics;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day09
{
  [Theory]
  [InlineData("Day09.Sample.txt", 50)]
  [InlineData("Day09.txt", 4763040296)]
  public void Part1(string inputFile, long expected)
  {
    var points = P.Long.Star(",").End().Select(it => new Point(it[0], it[1]))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    var pairs = points.Pairs().ToList();

    pairs.Select(p => (Math.Abs(p.First.X - p.Second.X) + 1) * (Math.Abs(p.First.Y - p.Second.Y) + 1))
      .Max().Should().Be(expected);
  }


  [Theory]
  [InlineData("Day09.Sample.txt", 24)]
  [InlineData("Day09.txt", 0)]
  public void Part2(string inputFile, long expected)
  {
    var points = P.Long.Star(",").End().Select(it => new Point(it[0], it[1]))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    HashSet<Point> occupied = [.. points.Windows(2).SelectMany(p => LineBetween(p[0], p[1]))];
    foreach (var p in LineBetween(points[0], points[^1])) occupied.Add(p);
    FloodFill(occupied);
    Console.WriteLine(occupied.Count);

    var ps = points.Pairs().ToList();
    long found = 0;
    foreach (var (p, index) in ps.WithIndices())
    {
      Console.WriteLine($"{index} / {ps.Count}");
      if (FullOverlap(p.First, p.Second, occupied))
      {
        found = Math.Max(found, (Math.Abs(p.First.X - p.Second.X) + 1) * (Math.Abs(p.First.Y - p.Second.Y) + 1));
      }
    }

    found.Should().Be(expected);
  }

  private static bool FullOverlap(Point first, Point Second, HashSet<Point> grid)
  {
    return Rectangle(first, Second).All(p => grid.Contains(p));
  }

  private static IEnumerable<Point> Rectangle(Point first, Point second)
  {
    HashSet<Point> result = [];
    for (var x = Math.Min(first.X, second.X); x <= Math.Max(first.X, second.X); x++)
      for (var y = Math.Min(first.Y, second.Y); y <= Math.Max(first.Y, second.Y); y++)
        yield return new(x, y);
  }

  private static void FloodFill(HashSet<Point> grid)
  {
    var minx = grid.Min(p => p.X);
    var miny = grid.Min(p => p.Y);
    var maxx = grid.Max(p => p.X);
    var maxy = grid.Max(p => p.Y);
    Point outside = new Point(minx - 1, miny + 1);
    while (!grid.Contains(outside))
    {
      if (outside.X > maxx) throw new ApplicationException();
      outside += Vector.East;
    }
    // we're on a line; move one more east
    outside += Vector.East;

    Queue<Point> open = new([outside]);
    while (open.TryDequeue(out var current))
    {
      if (current.X < minx || current.X > maxx || current.Y < miny || current.Y > maxy) throw new ApplicationException();
      foreach (var n in current.CardinalNeighbors())
      {
        if (!grid.Add(n)) continue;
      if (grid.Count % 10000 == 0) Console.WriteLine(grid.Count);
        open.Enqueue(n);
      }
    }
    Console.WriteLine(grid.Count);
  }

  private static IEnumerable<Point> LineBetween(Point first, Point second)
  {
    long y1 = first.Y;
    long y2 = first.Y;
    long x1 = first.X;
    long x2 = first.X;
    if (first.X == second.X)
    {
      y1 = Math.Min(first.Y, second.Y);
      y2 = Math.Max(first.Y, second.Y);
    }
    else if (first.Y == second.Y)
    {
      x1 = Math.Min(first.X, second.X);
      x2 = Math.Max(first.X, second.X);
    }
    else throw new ApplicationException("Points not on a line!");
    for (var x = x1; x <= x2; x++)
      for (var y = y1; y <= y2; y++)
        yield return new(x, y);
  }
}