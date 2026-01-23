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
  public void Part1(string inputFile, long expected)
  {
    var grid = GetInput(inputFile);

    Paint(grid).Should().Be(expected);
  }

  private long Paint(Grid<char> grid)
  {
    var first = new Point(0, 1);

    var unwatered = grid.Items().Where(it => it.Value == 'P').Select(it => it.Point).ToHashSet();
    Dictionary<Point, long> watered = [];

    Dictionary<Point, long> closed = [];
    closed[first] = 0;

    Queue<Point> open = new([first]);

    while (watered.Count < unwatered.Count && open.TryDequeue(out var current))
    {
      var d = closed[current];
      foreach(var neighbor in current.CardinalNeighbors())
      {
        var nv = grid.Get(neighbor, Wall);
        if (nv == Wall) continue;
        if (closed.TryGetValue(neighbor, out var existing) && existing <= d + 1) continue;
        if (nv == PalmTree) watered[neighbor] = d + 1;
        closed[neighbor] = d + 1;
        open.Enqueue(neighbor);
      }
    }

    return watered.Values.Max();
  }

  private static Grid<char> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }

}
