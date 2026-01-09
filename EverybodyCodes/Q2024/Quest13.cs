using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest13
{
  [Theory]
  [InlineData("Quest13.1.Sample.txt", 28)]
  [InlineData("Quest13.1.txt", 159)]
  [InlineData("Quest13.2.txt", 646)]
  [InlineData("Quest13.3.Sample.txt", 14)]
  [InlineData("Quest13.3.txt", 529)]
  public void Part1(string inputFile, long expected)
  {
    var world = GetInput(inputFile);

    FindPath(world).Should().Be(expected);
  }

  private long FindPath(World world)
  {
    var grid = world.Grid;
    var start = world.Goal;
    var goal = world.Starts.ToHashSet();

    PriorityQueue<(Point position, long steps)> open = new(it => it.steps);
    open.Enqueue((start, 0));
    Dictionary<Point, long> closed = [];
    closed[start] = 0;
    var min = long.MaxValue;

    while (open.TryDequeue(out var current))
    {
      if (goal.Contains(current.position))
      {
        return current.steps;
      }
      if (current.steps >= min) continue;
      foreach (var neighbor in Open(grid, current.position))
      {
        var d = current.steps + 1 + Elevate(grid[current.position], grid[neighbor]);
        if (d > min) continue;
        if (closed.GetValueOrDefault(neighbor, long.MaxValue) < d) continue;
        closed[neighbor] = d;
        if (goal.Contains(neighbor))
        {
          if (d < min) min = d;
          else continue;
        }
        open.Enqueue((neighbor, d));
      }
    }

    throw new ApplicationException();
  }

  private static long Elevate(int from, int to)
  {
    return new[] { Math.Abs(from - to), from + (10 - to), (10 - from) + to }.Min();
  }

  private IEnumerable<Point> Open(Grid<int> grid, Point position)
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

    return new(grid, [.. temp.Items().Where(it => it.Value == 'S').Select(it => it.Key)], temp.Items().Single(it => it.Value == 'E').Key);
  }

}
