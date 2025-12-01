
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using System.Data.Common;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day01
{
  [Theory]
  [InlineData("Day01.Sample.txt", 3)]
  [InlineData("Day01.txt", 1084)] // 1083 too low
  public void Part1(string inputFile, long expected)
  {
    var data = P.Format("{}{}", P.Choice("L", "R"), P.Long).ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    var count = 0L;
    var dial = 50L;

    foreach (var (First, Second) in data)
    {
      dial += Second * (First == "L" ? -1 : 1);
      dial = MathUtils.MathMod(dial, 100);
      if (dial == 0) count += 1;
    }

    count.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day01.Sample.txt", 6)]
  [InlineData("Day01.txt", 6475)] // 6467 too low, 6966 too high
  public void Part2(string inputFile, long expected)
  {
    var data = P.Format("{}{}", P.Choice("L", "R"), P.Long).ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    var count = 0L;
    var dial = 50L;

    foreach (var (direction, magnitude) in data)
    {
      if (direction == "L")
      {
        var x = magnitude;
        if (dial != 0 && x >= dial)
        {
          count += 1;
          x -= dial;
        }
        count += x / 100;
      }
      else
      {
        var x = magnitude;
        if (x >= (100-dial))
        {
          count += 1;
          x -= 100-dial;
        }
        count += x / 100;
      }
      dial += magnitude * (direction == "L" ? -1 : 1);
      dial = MathUtils.MathMod(dial, 100);
    }

    count.Should().Be(expected);
  }
}