
using FluentAssertions;
using Parser;
using Mng.Quest.CSharp.Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day04
{
  [Theory]
  [InlineData("Day04.Sample.txt", 13)]
  [InlineData("Day04.txt", 1467)]
  public void Part1(string inputFile, long expected)
  {
    var grid = AdventOfCode2025Loader.ReadLines(inputFile).Gridify();
    var actual = 0L;
    foreach (var (Key, Value) in grid.Items().Where(it => it.Value == '@'))
    {
      if (Key.CompassRoseNeighbors().Where(n => grid.TryGetValue(n, out var np) && np == '@').Count() < 4)
      {
        actual += 1;
      }
    }
    actual.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day04.Sample.txt", 43)]
  [InlineData("Day04.txt", 8484)]
  public void Part2ViaBF(string inputFile, long expected)
  {
    var grid = AdventOfCode2025Loader.ReadLines(inputFile).Gridify();
    var start = grid.Items().Count(it => it.Value == '@');
    while (true)
    {
      List<Point> candidates = [];
      foreach (var (Key, Value) in grid.Items().Where(it => it.Value == '@'))
      {
        if (Key.CompassRoseNeighbors().Where(n => grid.TryGetValue(n, out var np) && np == '@').Count() < 4)
        {
          candidates.Add(Key);
        }
      }
      if (candidates.Count == 0) break;
      foreach (var p in candidates) grid[p] = '.';
    }
    var final = grid.Items().Count(it => it.Value == '@');
    (start - final).Should().Be((int)expected);
  }

  [Theory]
  [InlineData("Day04.Sample.txt", 43)]
  [InlineData("Day04.txt", 8484)]
  public void Part2ViaForces(string inputFile, long expected)
  {
    var grid = AdventOfCode2025Loader.ReadLines(inputFile).Gridify();
    var start = grid.Items().Count(it => it.Value == '@');
    var forces = grid.Points().ToDictionary(p => p, p => p.CompassRoseNeighbors().Count(n => grid.Get(n, '.') == '@'));

    Queue<Point> open = new([.. forces.Where(it => it.Value < 4 && grid.Get(it.Key, '.')  == '@').Select(it => it.Key)]);

    while (open.TryDequeue(out var current))
    {
      grid[current] = '.';
      foreach (var n in current.CompassRoseNeighbors().Where(forces.ContainsKey))
      {
        forces[n] = forces[n] - 1;
        if (forces[n] == 3 && grid.Get(n, '.') == '@') open.Enqueue(n);
      }
    }
    var final = grid.Items().Count(it => it.Value == '@');
    (start - final).Should().Be((int)expected);
  }
}