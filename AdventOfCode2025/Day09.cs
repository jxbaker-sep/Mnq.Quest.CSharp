
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
  [InlineData("Day09.txt", 0)] // 4634026886 too high
  public void Part2(string inputFile, long expected)
  {
    var points = P.Long.Star(",").End().Select(it => new Point(it[0], it[1]))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    var vlines = points.Windows(2).Append([points[0], points[^1]])
      .Where(it => it[0].X == it[1].X)
      .Select(it => (top: Math.Min(it[0].Y, it[1].Y), bottom: Math.Max(it[0].Y, it[1].Y), x: it[0].X))
      .ToList();

    var hlines = points.Windows(2).Append([points[0], points[^1]])
      .Where(it => it[0].Y == it[1].Y)
      .Select(it => (left: Math.Min(it[0].X, it[1].X), right: Math.Max(it[0].X, it[1].X), y: it[0].Y))
      .ToList();

    var result = -1L;

    foreach (var pair in points.Pairs())
    {
      var minx = Math.Min(pair.First.X, pair.Second.X);
      var maxx = Math.Max(pair.First.X, pair.Second.X);
      var miny = Math.Min(pair.First.Y, pair.Second.Y);
      var maxy = Math.Max(pair.First.Y, pair.Second.Y);
      var found = true;
      foreach (var vertex in new[] { new Point(minx, miny), new Point(minx, maxy), new Point(maxx, miny), new Point(maxx, maxy) })
      {
        if (!PointInPolygon(vertex, vlines, hlines))
        {
          found = false;
          break;
        }
      }
      if (found) {
        result = Math.Max(result, (Math.Abs(pair.First.X - pair.Second.X) + 1) * (Math.Abs(pair.First.Y - pair.Second.Y) + 1));
      }
    }

    result.Should().Be(expected);
  }

  private static bool PointInPolygon(Point vertex, List<(long top, long bottom, long x)> vlines, List<(long left, long right, long y)> hlines)
  {
    foreach(var (left, right, y) in hlines) 
      if (y == vertex.Y && left <= vertex.X && vertex.X <= right)
        return true;

    foreach(var (top, bottom, x) in vlines) 
      if (x == vertex.X && top <= vertex.Y && vertex.Y <= bottom)
        return true;

    return vlines.Count(line => line.x <= vertex.X && line.top <= vertex.Y && vertex.Y < line.bottom) % 2 == 1;
  }
}
