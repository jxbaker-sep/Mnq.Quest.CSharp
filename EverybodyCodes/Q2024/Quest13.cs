using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest13
{
  // [Theory]
  // [InlineData("Quest13.1.Sample.txt", 28)]
  // [InlineData("Quest13.1.txt", 159)]
  // [InlineData("Quest13.2.txt", 646)]
  // [InlineData("Quest13.3.Sample.txt", 14)]
  // [InlineData("Quest13.3.txt", 529)]
  // public void Part1(string inputFile, long expected)
  // {
  //   var world = GetInput(inputFile);

  //   FindPath(world).Should().Be(expected);
  // }

  [Theory]
  [InlineData("Quest13.1.Sample.txt", 28)]
  [InlineData("Quest13.1.txt", 159)]
  [InlineData("Quest13.2.txt", 646)]
  [InlineData("Quest13.3.Sample.txt", 14)]
  [InlineData("Quest13.3.txt", 529)]
  public void Painterly(string inputFile, long expected)
  {
    var world = GetInput(inputFile);

    FindPainterlyPath(world).Should().Be(expected);
  }

  private static long FindPainterlyPath(World world)
  {
    var grid = world.Grid;
    var start = world.Goal;
    var goal = world.Starts.ToHashSet();

    Dictionary<Point, long> closed = [];
    closed[start] = 0;
    PriorityQueue<Point> open = new(it => closed[it]);
    open.Enqueue(start);

    while (open.TryDequeue(out var current))
    {
      var currentSteps = closed[current];
      foreach (var neighbor in Open(grid, current))
      {
        var d = currentSteps + 1 + Elevate(grid[current], grid[neighbor]);
        if (closed.GetValueOrDefault(neighbor, long.MaxValue) <= d) continue;
        closed[neighbor] = d;
        open.Enqueue(neighbor);
      }
    }

    return goal.Min(g => closed[g]);
  }

  // private static long FindPath(World world)
  // {
  //   var grid = world.Grid;
  //   var start = world.Goal;
  //   var goal = world.Starts.ToHashSet();

  //   PriorityQueue<(Point position, long steps)> open = new(it => it.steps);
  //   open.Enqueue((start, 0));
  //   Dictionary<Point, long> closed = [];
  //   closed[start] = 0;

  //   while (open.TryDequeue(out var current))
  //   {
  //     if (goal.Contains(current.position))
  //     {
  //       return current.steps;
  //     }
  //     foreach (var neighbor in Open(grid, current.position))
  //     {
  //       var d = current.steps + 1 + Elevate(grid[current.position], grid[neighbor]);
  //       if (closed.GetValueOrDefault(neighbor, long.MaxValue) < d) continue;
  //       closed[neighbor] = d;
  //       open.Enqueue((neighbor, d));
  //     }
  //   }

  //   throw new ApplicationException();
  // }

  private static long Elevate(int from, int to)
  {
    return new[] { Math.Abs(from - to), from + (10 - to), (10 - from) + to }.Min();
  }

  private static IEnumerable<Point> Open(Grid<int> grid, Point position)
  {
    foreach (var neighbor in position.CardinalNeighbors())
    {
      if (grid.Get(neighbor, Wall) == Wall) continue;
      yield return neighbor;
    }
  }

  const int Wall = -1;
  record World(Grid<int> Grid, List<Point> Starts, Point Goal);
  private static World GetInput(string inputFile)
  {
    var temp = ECLoader.ReadLines(inputFile).Gridify();
    var grid = ECLoader.ReadLines(inputFile).Gridify(it => it switch { '#' => Wall, ' ' => Wall, 'S' => 0, 'E' => 0, _ => it - '0' });

    return new(grid, [.. temp.Items().Where(it => it.Value == 'S').Select(it => it.Point)], temp.Items().Single(it => it.Value == 'E').Point);
  }

}
