using FluentAssertions;
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

    Solve1(grid).Should().Be(expected);
  }


  static long Solve1(Grid<char> grid)
  {
    var start = grid.Items().Single(it => it.Point.Y == 0 && it.Value == '.').Point;
    var goal = grid.Items().Select(it => it.Value).Where(h => h != Wall && h != Lake).ToHashSet().Select(it => $"{it}").OrderBy(it => it).Join("");
    Dictionary<(Point Point, string Key), long> closed = [];
    closed[(start, "")] = 0;
    Queue<(Point Point, string Key)> open = [];
    open.Enqueue((start, ""));

    while (open.TryDequeue(out var current))
    {
      foreach (var neighbor in current.Point.CardinalNeighbors())
      {
        var d = closed[current] + 1;
        var cell = grid.Get(neighbor, Wall);
        if (cell == Wall || cell == Lake) continue;
        if (neighbor == start && current.Key != goal) continue;
        if (neighbor == start && current.Key == goal) return d;
        var nextKey = (current.Key + cell).ToHashSet().Select(it => $"{it}").OrderBy(it => it).Join("");
        if (closed.GetValueOrDefault((neighbor, nextKey), long.MaxValue) <= d) continue;
        closed[(neighbor, nextKey)] = d;
        open.Enqueue((neighbor, nextKey));
      }
    }

    // return closed[(start, goal)];
    throw new ApplicationException();
  }

  private static Grid<char> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }

}
