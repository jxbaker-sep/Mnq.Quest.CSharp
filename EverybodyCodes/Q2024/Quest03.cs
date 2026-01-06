using Parser;
using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using System.Data;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest03
{
  [Theory]
  [InlineData("Quest03.Sample.1.txt", 35)]
  [InlineData("Quest03.1.txt", 134)]
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    long sum = 0;
    while (true) {
      var value = Dig(input);
      if (value == 0) break;
      sum += value;
    }
    sum.Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest03.2.txt", 2699)]
  public void Part2(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    long sum = 0;
    while (true) {
      var value = Dig(input);
      if (value == 0) break;
      sum += value;
    }
    sum.Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest03.Sample.3.txt", 29)]
  [InlineData("Quest03.3.txt", 10322)]
  public void Part3(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    long sum = 0;
    while (true) {
      var value = Dig(input, true);
      if (value == 0) break;
      sum += value;
    }
    sum.Should().Be(expected);
  }

  private static long Dig(Dictionary<Point, long> grid, bool includeDiagonals = false)
  {
    long changes = 0;

    foreach(var (point, height) in grid.ToList())
    {
      if ((includeDiagonals ? point.CompassRoseNeighbors() : point.CardinalNeighbors()).All(neighbor => grid.GetValueOrDefault(neighbor) >= height))
      {
        grid[point] += 1;
        changes += 1;
      }
    }

    return changes;
  }

  private static Dictionary<Point, long> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify()
      .Items()
      .Where(it => it.Value == '#')
      .ToDictionary(it => it.Key, it => 0L);
  }
}
