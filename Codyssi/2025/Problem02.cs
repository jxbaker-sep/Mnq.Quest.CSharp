using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem02
{
  [Theory]
  [InlineData("Problem02.Sample.txt", 19)]
  [InlineData("Problem02.txt", 66502)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    input.WithIndices().Where(it => it.Value).Select(it => it.Index + 1).Sum()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem02.Sample.txt", 2)]
  [InlineData("Problem02.txt", 130)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    input.WithIndices()
      .GroupBy(it => it.Index / 2, it => it.Value)
      .Select(group => group.Take(2).Aggregate((a,b) => group.Key % 2 == 0 ? a && b : a || b) )
      .Count(it => it)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem02.Sample.txt", 7)]
  [InlineData("Problem02.txt", 504)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    Summarize(input).Should().Be(expected);
  }

  private long Summarize(List<bool> input)
  {
    if (input.Count() == 0) return 0;
    if (input.Count() == 1) return input[0] ? 1 : 0;
    return input.Count(it => it) + 
      Summarize(input.WithIndices()
        .GroupBy(it => it.Index / 2, it => it.Value)
        .Select(group => group.Take(2).Aggregate((a,b) => group.Key % 2 == 0 ? a && b : a || b) )
        .ToList()
      );
  }

  private static List<bool> GetInput(string inputFile)
  {
    return P.Choice("FALSE", "TRUE")
      .Select(it => it == "TRUE")
      .ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}
