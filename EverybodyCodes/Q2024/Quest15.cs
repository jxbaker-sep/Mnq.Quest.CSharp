using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.Z3;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest15
{
  const char Wall = '#';
  const char Lake = '~';
  const char Open = '.';

  [Theory]
  [InlineData("Quest15.1.Sample.txt", 26)]
  [InlineData("Quest15.1.txt", 204)]
  [InlineData("Quest15.2.Sample.txt", 38)]
  [InlineData("Quest15.2.txt", 520)]
  public void Part1(string inputFile, int expected)
  {
    var grid = GetInput(inputFile);

    Solve2(grid).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest15.3.txt", 1510)]
  public void Part3(string inputFile, int expected)
  {
    var grid = GetInput(inputFile);

    Solve2(grid).Should().Be(expected);
  }

  long Solve2(Grid<char> grid)
  {
    var start = grid.Items().Single(it => it.Point.Y == 0 && it.Value == '.').Point;
    grid[start] = 'S';
    var herbSpaces = grid.Items().Where(h => h.Value != Wall && h.Value != Lake && h.Value != Open).ToHashSet();
    var herbs = herbSpaces.Select(it => it.Value).ToHashSet().Select(it => $"{it}").OrderBy(it => it).Join();
    Dictionary<Point, Dictionary<char, List<(Point Point, long Distance)>>> herbDistances = [];

    foreach (var herbSpace in herbSpaces)
    {
      herbDistances[herbSpace.Point] = Solve1(grid, herbSpace.Point);
    }

    return Solve3(herbDistances, herbs, start);
  }

  private readonly Dictionary<(Point, string), long> Cache = [];
  private long Solve3(Dictionary<Point, Dictionary<char, List<(Point Point, long Distance)>>> herbDistances, string herbs, Point start)
  {
    if (herbs.Length == 0) return 0;
    var key = (start, herbs);
    if (Cache.TryGetValue((key), out var needle)) return needle;

    if (herbs == "S")
    {
      var result = herbDistances[start]['S'].Min(it => it.Distance);
      Cache[key] = result;
      return result;
    }

    var found = long.MaxValue;
    foreach (var remainingHerb in herbs.Where(it => it != 'S'))
    {
      var nextHerbs = herbs.Where(it => it != remainingHerb).Select(it => $"{it}").Join();
      foreach (var herbPoint in herbDistances[start][remainingHerb])
      {
        if (herbPoint.Distance >= found) continue;
        var recursive = Solve3(herbDistances, nextHerbs, herbPoint.Point);
        found = Math.Min(found, recursive + herbPoint.Distance);
      }
    }

    Cache[key] = found;
    Console.WriteLine(Cache.Count);
    return found;
  }


  [Theory]
  [InlineData("Quest15.1.Sample.txt", 26)]
  [InlineData("Quest15.1.txt", 204)]
  [InlineData("Quest15.2.Sample.txt", 38)]
  [InlineData("Quest15.2.txt", 520)]
  public void Part4(string inputFile, int expected)
  {
    var grid = GetInput(inputFile);

    Solve5(grid).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest15.3.txt", 1510)]
  public void Part5(string inputFile, int expected)
  {
    var grid = GetInput(inputFile);

    Solve5(grid).Should().Be(expected);
  }

  private long Solve5(Grid<char> grid)
  {
    var start = grid.Items().Single(it => it.Point.Y == 0 && it.Value == '.').Point;
    grid[start] = 'S';
    var points = grid.Items().Where(h => h.Value != Wall && h.Value != Lake && h.Value != Open).GroupToDictionary(it => it.Value, it => it.Point);
    var herbs = points.Keys.OrderBy(it => it).Join();
    Dictionary<(Point, Point), long> distances = [];

    foreach(var point in points.SelectMany(it => it.Value))
    {
      var temp = PointToPoints(grid, point);
      foreach(var p in temp) distances[p.Key] = p.Value;
    }

    return Solve4(distances, points, herbs).Min(it => it.LaterDistance + distances[(it.Point, start)]);
  }

  private readonly Dictionary<string, List<(Point Point, long LaterDistance)>> Cache2 = [];
  private List<(Point Point, long LaterDistance)> Solve4(Dictionary<(Point, Point), long> distances,
    Dictionary<char, List<Point>> points, string herbs)
  {
    if (herbs == "") throw new ApplicationException();
    if (herbs == "S") return [(points['S'][0], 0)];
    if (Cache2.TryGetValue(herbs, out var needle)) return needle;

    List<(Point Point, long LaterDistance)> result = [];
    foreach (var herb in herbs.Where(it => it != 'S'))
    {
      var remainder = herbs.Where(it => it != herb).Join();
      var recursive = Solve4(distances, points, remainder);
      foreach (var point in points[herb])
      {
        result.Add((point, recursive.Min(r2 => distances[(r2.Point, point)] + r2.LaterDistance)));
      }
    }

    // Console.WriteLine($"{herbs} {result.Count}");
    Cache2[herbs] = result;
    return result;
  }

  static long Solve(Grid<char> grid)
  {
    var start = grid.Items().Single(it => it.Point.Y == 0 && it.Value == '.').Point;
    grid[start] = 'S';
    var herbSpaces = grid.Items().Where(h => h.Value != Wall && h.Value != Lake && h.Value != Open).ToHashSet();
    var herbs = herbSpaces.Select(it => it.Value).ToHashSet().Select(it => $"{it}").OrderBy(it => it).Join();
    Dictionary<Point, Dictionary<char, List<(Point Point, long Distance)>>> herbDistances = [];

    foreach (var herbSpace in herbSpaces)
    {
      herbDistances[herbSpace.Point] = Solve1(grid, herbSpace.Point);
    }

    PriorityQueue<(Point Point, string RemainingHerbs, long Distance)> open = new(it => it.Distance + it.Point.ManhattanDistance(start) + it.RemainingHerbs.Length - 1);
    open.Enqueue((start, herbs, 0));

    Dictionary<(Point Point, string RemainingHerbs), long> closed = [];
    closed[(start, herbs)] = 0;

    // long found = long.MaxValue;

    while (open.TryDequeue(out var current))
    {
      // Console.WriteLine($"{found} {open.Count}");
      if (current.RemainingHerbs == "") return current.Distance;
      var goingHome = current.RemainingHerbs == "S";
      // if (current.Distance >= found) continue;
      foreach (var remainingHerb in current.RemainingHerbs.Where(it => goingHome || it != 'S'))
      {
        var nextKey = current.RemainingHerbs.Where(it => it != remainingHerb).Select(it => $"{it}").Join();
        foreach (var herbPoint in herbDistances[current.Point][remainingHerb])
        {
          var d = current.Distance + herbPoint.Distance;
          // if (d >= found) continue;
          if (closed.GetValueOrDefault((herbPoint.Point, nextKey), long.MaxValue) <= d) continue;
          closed[(herbPoint.Point, nextKey)] = d;
          if (nextKey == "")
          {
            // return d;
            // found = Math.Min(found, d);

            // continue;
          }
          open.Enqueue((herbPoint.Point, nextKey, d));
        }
      }
    }

    // return found;
    throw new ApplicationException();
  }

  static Dictionary<char, List<(Point Point, long Distance)>> Solve1(Grid<char> grid, Point start)
  {
    Dictionary<char, List<(Point Point, long Distance)>> result = [];
    Dictionary<Point, long> closed = [];
    closed[start] = 0;
    Queue<Point> open = [];
    open.Enqueue(start);

    while (open.TryDequeue(out var current))
    {
      foreach (var neighbor in current.CardinalNeighbors())
      {
        var d = closed[current] + 1;
        var cell = grid.Get(neighbor, Wall);
        if (cell == Wall || cell == Lake) continue;
        if (closed.GetValueOrDefault(neighbor, long.MaxValue) <= d) continue;
        closed[neighbor] = d;
        open.Enqueue(neighbor);
        result[cell] = result.GetValueOrDefault(cell) ?? [];
        result[cell].Add((neighbor, d));
      }
    }

    return result;
  }

  

  static Dictionary<(Point, Point), long> PointToPoints(Grid<char> grid, Point start)
  {
    Dictionary<(Point, Point), long> result = [];
    Dictionary<Point, long> closed = [];
    closed[start] = 0;
    Queue<Point> open = [];
    open.Enqueue(start);

    while (open.TryDequeue(out var current))
    {
      foreach (var neighbor in current.CardinalNeighbors())
      {
        var d = closed[current] + 1;
        var cell = grid.Get(neighbor, Wall);
        if (cell == Wall || cell == Lake) continue;
        if (closed.GetValueOrDefault(neighbor, long.MaxValue) <= d) continue;
        closed[neighbor] = d;
        open.Enqueue(neighbor);
        if (cell != Open)
        {
          result[(start, neighbor)] = d;
        }
      }
    }

    return result;
  }

  private static Grid<char> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }

}
