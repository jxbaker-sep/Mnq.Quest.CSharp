using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;
using Mnq.Quest.CSharp.EverybodyCodes;
using System.Net.Sockets;
using System.Xml;
using System.Linq.Expressions;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest03
{
  [Theory]
  [InlineData("Quest03.Sample.1.txt", 35)]
  [InlineData("Quest03.1.txt", 0)]
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

  private static long Dig(Dictionary<Point, long> grid)
  {
    long changes = 0;

    foreach(var (point, height) in grid.ToList())
    {
      if (point.CardinalNeighbors().All(neighbor => grid.GetValueOrDefault(neighbor) >= height))
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
