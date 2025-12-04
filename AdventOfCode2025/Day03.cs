
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day03
{
  [Theory]
  [InlineData("Day03.Sample.txt", 357)]
  [InlineData("Day03.txt", 17_281)]
  public void Part1(string inputFile, long expected)
  {
    var data = P.Digit.Select(it => (long)(it - '0')).Star().ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    data.Select(line =>
    {
      return FindMaxJoltage(line, 2);
    }).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day03.Sample.txt", 3121910778619)]
  [InlineData("Day03.txt", 171388730430281)]
  public void Part2(string inputFile, long expected)
  {
    var data = P.Digit.Select(it => (long)(it - '0')).Star().ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    data.Select(line =>
    {
      return FindMaxJoltage(line, 12);
    }).Sum().Should().Be(expected);
  }

  static long FindMaxJoltage(List<long> line, int take)
  {
    long joltage = 0;
    var index = 0;
    var withIndexes = line.WithIndices().ToList();
    for (int i = take - 1; i >= 0; i--)
    {
      var j = withIndexes[(index)..(line.Count - i)].MaxBy(it => it.Value);
      joltage = joltage * 10 + j.Value;
      index = j.Index + 1;
    }
    return joltage;
  }
}