using System.Diagnostics;
using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest18
{

  const char PalmTree = 'P';
  const char Wall = '#';
  const char Open = '.';

  [Theory]
  [InlineData("Quest18.1.Sample.txt", 11)]
  [InlineData("Quest18.1.txt", 119)]
  [InlineData("Quest18.2.Sample.txt", 21)]
  [InlineData("Quest18.2.txt", 1837)]
  public void Part1(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    List<Point> firsts = [.. grid.Points((point, value) => value == Open && (point.X == 0 || point.X == grid.Width - 1))];

    Paint(grid, firsts).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest18.3.Sample.txt", 12)]
  [InlineData("Quest18.3.txt", 248_879)] // 65.3s
  public void Part3(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    long min = long.MaxValue;
    List<Point> firsts = [.. grid.Points((point, value) => value == Open)];
    var i = 0;
    Stopwatch sw = new();
    sw.Start();
    foreach (var first in firsts)
    {
      i += 1;
      var item = Paint(grid, [first], min, true);
      Console.WriteLine($"{i}/{firsts.Count} {sw.ElapsedMilliseconds} {firsts.Count * sw.ElapsedMilliseconds / i}");
      min = Math.Min(min, item);
    }

    min.Should().Be(expected);
  }

  private static long Paint(Grid<char> grid, List<Point> firsts, long maxValue = long.MaxValue, bool wantSum = false)
  {
    var allPlants = grid.Points((_, value) => value == 'P').ToHashSet();
    Dictionary<Point, long> watered = [];

    Dictionary<Point, long> closed = firsts.ToDictionary(it => it, _ => 0L);

    Queue<Point> open = new(firsts);
    var remaining = allPlants.Count;
    long currentSum = 0;

    while (watered.Count < allPlants.Count && open.TryDequeue(out var current))
    {
      var d = closed[current];
      foreach (var neighbor in current.CardinalNeighbors())
      {
        var nv = grid.Get(neighbor, Wall);
        if (nv == Wall) continue;
        if (closed.TryGetValue(neighbor, out var existing) && existing <= d + 1) continue;
        if (nv == PalmTree)
        {
          if (!watered.ContainsKey(neighbor)) remaining -= 1;
          watered[neighbor] = d + 1;
          currentSum = watered.Values.Sum();
          if (remaining == 0)
          {
            if (!wantSum) return watered.Values.Max();
            return watered.Values.Sum();
          }
        }
        closed[neighbor] = d + 1;
        open.Enqueue(neighbor);
        var estimatedSum = currentSum + (d + 2) * remaining;
          ;
        if (estimatedSum >= maxValue) return maxValue;
      }
    }

    throw new ApplicationException();
  }

  private static Grid<char> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }

}
