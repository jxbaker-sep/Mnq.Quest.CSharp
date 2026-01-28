using System.Diagnostics;
using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Utils;

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

    Paint(grid).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest18.3.Sample.txt", 12)]
  [InlineData("Quest18.3.txt", 248_879)] // 65.3s
  public void Part3(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    Paint2(grid).Should().Be(expected);
  }

  private static long Paint(Grid<char> grid)
  {
    List<Point> firsts = [.. grid.Points((point, value) => value == Open && (point.X == 0 || point.X == grid.Width - 1))];

    var allPlants = grid.Points((_, value) => value == 'P').ToHashSet();
    Dictionary<Point, long> watered = [];

    Dictionary<Point, long> closed = firsts.ToDictionary(it => it, _ => 0L);

    Queue<Point> open = new(firsts);
    var remaining = allPlants.Count;

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
          if (remaining == 0)
          {
            return watered.Values.Max();
          }
        }
        closed[neighbor] = d + 1;
        open.Enqueue(neighbor);
      }
    }

    throw new ApplicationException();
  }

  private static long Paint2(Grid<char> grid)
  {
    List<Point> firsts = [.. grid.Points((point, value) => value == Open)];

    LinkedList<IEnumerator<(long, bool)>> searches = [];

    foreach(var first in firsts)
    {
      var x = PaintStepable(grid, first).GetEnumerator();
      searches.AddLast(x);
    }

    long found = long.MaxValue;
    while (searches.Count > 0)
    {
      for(var search = searches.First; search != null; )
      {
        if (!search.Value.MoveNext()) throw new ApplicationException();
        var (estimate, done) = search.Value.Current;
        if (done)
        {
          found = Math.Min(found, estimate);
          var temp = search.Next;
          searches.Remove(search);
          search = temp;
          continue;
        }
        if (estimate >= found)
        {
          var temp = search.Next;
          searches.Remove(search);
          search = temp;
          continue;
        }
        search = search.Next;
      }
    }

    return found;
  }

  private static IEnumerable<(long Estimate, bool Done)> PaintStepable(Grid<char> grid, Point first)
  {
    var allPlants = grid.Points((_, value) => value == 'P').ToHashSet();
    Dictionary<Point, long> watered = [];

    Dictionary<Point, long> closed = [];
    closed[first] = 0;

    Queue<Point> open = new([first]);
    var unwatered = allPlants.Count;
    var sofar = 0L;

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
          if (!watered.ContainsKey(neighbor)) unwatered -= 1;
          watered[neighbor] = d + 1;
          sofar = watered.Values.Sum();
          if (unwatered == 0)
          {
            yield return (sofar, true);
            yield break;
          }
        }
        closed[neighbor] = d + 1;
        open.Enqueue(neighbor);
      }
      yield return (sofar + unwatered * (d + 1), false);
    }

    throw new ApplicationException();
  }

  private static Grid<char> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }

}
