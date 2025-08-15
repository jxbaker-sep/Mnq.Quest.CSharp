using Mng.Quest.CSharp.Utils;
using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;

namespace Mnq.Quest.CSharp.Codyssi;

public class CyclopsChaos
{
  [Theory]
  [InlineData("CyclopsChaos.Sample.txt", 73)]
  [InlineData("CyclopsChaos.txt", 204)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    input.Select(l => l.Sum()).Concat(input.Transpose().Select(l => l.Sum())).Min().Should().Be(expected);
  }

  [Theory]
  [InlineData("CyclopsChaos.Sample.txt", 94)]
  [InlineData("CyclopsChaos.txt", 90)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    Queue<Point> open = [];
    open.Enqueue(Point.Zero);
    Dictionary<Point, int> closed = [];
    closed[Point.Zero] = input[0][0];

    IEnumerable<Point> Neighbors(Point point) {
      if (point.X < 14) yield return point with {X = point.X + 1};
      if (point.Y < 14) yield return point with {Y = point.Y + 1};
    }

    while (open.TryDequeue(out var current))
    {
      var d = closed[current];
      foreach(var next in Neighbors(current))
      {
        var w = input[(int)next.Y][(int)next.X];
        if (closed.TryGetValue(next, out var needle) && needle <= w + d) continue;
        closed[next] = d + w;
        open.Enqueue(next);
      }
    }

    closed[new(14,14)].Should().Be(expected);
  }

  [Theory]
  [InlineData("CyclopsChaos.Sample.txt", 120)]
  [InlineData("CyclopsChaos.txt", 305)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    int maxx = input[0].Count - 1;
    int maxy = input.Count - 1;
    Queue<Point> open = [];
    open.Enqueue(Point.Zero);
    Dictionary<Point, int> closed = [];
    closed[Point.Zero] = input[0][0];

    IEnumerable<Point> Neighbors(Point point) {
      if (point.X < maxx) yield return point with {X = point.X + 1};
      if (point.Y < maxy) yield return point with {Y = point.Y + 1};
    }

    while (open.TryDequeue(out var current))
    {
      var distance = closed[current];
      foreach(var next in Neighbors(current))
      {
        var weight = input[(int)next.Y][(int)next.X];
        if (closed.TryGetValue(next, out var needle) && needle <= distance + weight) continue;
        closed[next] = distance + weight;
        open.Enqueue(next);
      }
    }

    closed[new(maxx, maxy)].Should().Be(expected);
  }

  private static List<List<int>> GetInput(string inputFile)
  {
    return P.Int.Trim().Star().ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}