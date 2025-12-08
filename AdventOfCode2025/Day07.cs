
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using System.Runtime.CompilerServices;
using Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day07
{
  [Theory]
  [InlineData("Day07.Sample.txt", 21)]
  [InlineData("Day07.txt", 1635)]
  public void Part1(string inputFile, long expected)
  {
    var grid = AdventOfCode2025Loader.ReadLines(inputFile).Gridify();

    var start = grid.Items().Single(it => it.Value == 'S').Key;

    Queue<Point> open = new([start + Vector.South]);
    var splits = 0L;
    while (open.TryDequeue(out var current))
    {
      if (!grid.Contains(current)) continue;
      if (grid[current] != '.') continue;
      while (grid.Contains(current) && grid[current] == '.')
      {
        grid[current] = '|';
        current += Vector.South;
      }

      if (grid.Contains(current) && grid[current] == '^')
      {
        splits += 1;
        open.Enqueue(current + Vector.West);
        open.Enqueue(current + Vector.East);
      }
    }
    splits.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day07.Sample.txt", 40)]
  [InlineData("Day07.txt", 58_097_428_661_390)]
  public void Part2(string inputFile, long expected)
  {
    var grid = AdventOfCode2025Loader.ReadLines(inputFile).Gridify();

    var start = grid.Items().Single(it => it.Value == 'S').Key;

    Queue<Point> open = new([start + Vector.South]);

    Journeys(grid, start + Vector.South).Should().Be(expected);
  }

  readonly Dictionary<Point, long> Cache = [];
  private long Journeys(Grid<char> grid, Point point)
  {
    if (Cache.TryGetValue(point, out var needle)) return needle;
    var current = point;
    while (grid.Contains(current) && grid[current] == '.')
    {
      current += Vector.South;
    }
    long result = 1;
    if (grid.Contains(current) && grid[current] == '^')
    {
      result = Journeys(grid, current + Vector.West) +
               Journeys(grid, current + Vector.East);
    }
    Cache[point] = result;
    return result;
  }
}