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
  public void Part1(string inputFile, long expected)
  {
    var world = GetInput(inputFile);

    FindPath(world).Should().Be(expected);
  }

  private long FindPath(World world)
  {
    var (grid, start, goal) = world;

    PriorityQueue<(Point position, HashSet<Point> closed, long steps)> open = new(it => it.steps);
    open.Enqueue((start, [start], 0));
    Dictionary<Point, long> closed = [];
    closed[start] = 0;

    while (open.TryDequeue(out var current))
    {
      foreach (var neighbor in Open(grid, current.position))
      {
        if (current.closed.Contains(neighbor)) continue;
        var d = current.steps + 1 + Elevate(grid[current.position], grid[neighbor]);
        if (closed.GetValueOrDefault(neighbor, long.MaxValue) < d) continue;
        closed[neighbor] = d;
        if (neighbor == goal) return d;
        open.Enqueue((neighbor, [..current.closed, neighbor], d));
      }
    }

    throw new ApplicationException();
  }

  private static long Elevate(int from, int to)
  {
    return new[]{Math.Abs(from - to), from + (10 - to), (10 - from) + to}.Min();
  }

  private IEnumerable<Point> Open(Grid<int> grid, Point position)
  {
    foreach(var neighbor in position.CardinalNeighbors())
    {
      if (grid.Get(neighbor, Wall) == Wall) continue;
      yield return neighbor;
    }
  }

  const int Wall = -1;
  record World(Grid<int> Grid, Point Start, Point Goal);
  private static World GetInput(string inputFile)
  {
    var temp = ECLoader.ReadLines(inputFile).Gridify();
    var grid = ECLoader.ReadLines(inputFile).Gridify(it => it switch{'#' => Wall, ' ' => Wall, 'S' => 0, 'E' => 0,  _ => it - '0'});

    return new(grid, temp.Items().Single(it => it.Value == 'S').Key, temp.Items().Single(it => it.Value == 'E').Key);
  }

}
