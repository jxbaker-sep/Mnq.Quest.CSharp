using Parser;
using FluentAssertions;
using Utils;
using System.Data;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest01
{
  [Theory]
  [InlineData("Quest01.Sample.txt", 5)]
  [InlineData("Quest01.txt", 1306)]
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    Compute(input, 1).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest01.Sample.2.txt", 28)]
  [InlineData("Quest01.2.txt", 5636)]
  public void Part2(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    Compute(input, 2).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest01.Sample.3.txt", 30)]
  [InlineData("Quest01.3.txt", 27983)]
  public void Part3(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    
    Compute(input, 3).Should().Be(expected);
  }

  long Compute(string input, int groupsOf)
  {
    var d = new Dictionary<char, long>();
    d['A'] = 0;
    d['B'] = 1;
    d['C'] = 3;
    d['D'] = 5;
    List<long> ads = [0, 0, 2, 6];
    return input.InGroupsOf(groupsOf)
      .Select(it => it.Where(a => a != 'x').ToList())
      .Select(it =>
        it.Select(a => d[a]).Sum() + ads[it.Count]
      )
      .Sum();
  }

  private static string GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile)[0];
  }
}
