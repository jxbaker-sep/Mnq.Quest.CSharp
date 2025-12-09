
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
  [InlineData("Day09.txt", 1396494456)] // 4634026886 too high, 443075248 too low
                                        //  1396494456
  public void Part2(string inputFile, long expected)
  {
    var points = P.Long.Star(",").End().Select(it => new Point(it[0], it[1]))
      .ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    var vlines = points.Windows(2).Append([points[0], points[^1]])
      .Where(it => it[0].X == it[1].X)
      .Select(it => (top: Math.Min(it[0].Y, it[1].Y), bottom: Math.Max(it[0].Y, it[1].Y), x: it[0].X))
      .OrderBy(it => it.x)
      .ToList();

    var hlines = points.Windows(2).Append([points[0], points[^1]])
      .Where(it => it[0].Y == it[1].Y)
      .Select(it => (left: Math.Min(it[0].X, it[1].X), right: Math.Max(it[0].X, it[1].X), y: it[0].Y))
      .ToList();

    var result = -1L;

    var pairs = points.Pairs().ToList();
    foreach (var (First, Second) in pairs)
    {
      var minx = Math.Min(First.X, Second.X);
      var maxx = Math.Max(First.X, Second.X);
      var miny = Math.Min(First.Y, Second.Y);
      var maxy = Math.Max(First.Y, Second.Y);
      var upperLeft = new Point(minx, miny);
      var upperRight = new Point(maxx, miny);
      var lowerLeft = new Point(minx, maxy);
      var lowerRight = new Point(maxx, maxy);
      // var found = true;
      var found = LineInPolygon(upperLeft, upperRight, vlines, hlines) && LineInPolygon(upperRight, lowerRight, vlines, hlines) && LineInPolygon(lowerLeft, lowerRight, vlines, hlines) && LineInPolygon(lowerLeft, upperLeft, vlines, hlines);
      if (found)
      {
        var area = (Math.Abs(First.X - Second.X) + 1) * (Math.Abs(First.Y - Second.Y) + 1);
        result = Math.Max(result, area);
      }
    }

    result.Should().Be(expected);
  }

  private static bool LineInPolygon(Point First, Point Second, List<(long top, long bottom, long x)> vlines, List<(long left, long right, long y)> hlines)
  {
    var minx = Math.Min(First.X, Second.X);
    var maxx = Math.Max(First.X, Second.X);
    var miny = Math.Min(First.Y, Second.Y);
    var maxy = Math.Max(First.Y, Second.Y);
    if (!PointInPolygon(new(minx, miny), vlines, hlines) || !PointInPolygon(new(minx, maxy), vlines, hlines) || !PointInPolygon(new(maxx, miny), vlines, hlines) || !PointInPolygon(new(maxx, maxy), vlines, hlines)) return false;


    if (AnyLinesIntersect(vlines, hlines, minx, maxx, miny, maxy)) return false;

    return true;
  }

  private static bool AnyLinesIntersect(List<(long top, long bottom, long x)> vlines, List<(long left, long right, long y)> hlines, long minx, long maxx, long miny, long maxy)
  {
    if (minx > maxx || miny > maxy) return false;
    if (vlines.Any(line => line.x > minx && line.x < maxx && line.top < miny && miny < line.bottom)) return true;
    if (vlines.Any(line => line.x > minx && line.x < maxx && line.top < maxy && maxy < line.bottom)) return true;
    if (hlines.Any(line => line.y > miny && line.y < maxy && line.left < minx && minx < line.right)) return true;
    if (hlines.Any(line => line.y > miny && line.y < maxy && line.left < maxx && maxx < line.right)) return true;
    return false;
  }

  private static bool PointInPolygon(Point vertex, List<(long top, long bottom, long x)> vlines, List<(long left, long right, long y)> hlines)
  {
    foreach (var (left, right, y) in hlines)
      if (y == vertex.Y && left <= vertex.X && vertex.X <= right)
        return true;

    foreach (var (top, bottom, x) in vlines)
      if (x == vertex.X && top <= vertex.Y && vertex.Y <= bottom)
        return true;

    return vlines.TakeWhile(line => line.x <= vertex.X).Count(line => line.top <= vertex.Y && vertex.Y < line.bottom) % 2 == 1;
  }
}
