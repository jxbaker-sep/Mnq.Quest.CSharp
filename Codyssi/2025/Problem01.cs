using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem01
{
  [Theory]
  [InlineData("Problem01.Sample.txt", 2895391)]
  [InlineData("Problem01.txt", 147911509L)]
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    input.Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem01.Sample.txt", 2, 1261624)]
  [InlineData("Problem01.txt", 20, 128730260)]
  public void Part2(string inputFile, int discounted, long expected)
  {
    var input = GetInput(inputFile);
    input.OrderByDescending(it => it).Skip(discounted).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem01.Sample.txt", 960705)]
  [InlineData("Problem01.txt", 1407739)]
  public void Part3(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    input.Select((value, index) => value * (index % 2 == 0 ? 1 : -1)).Sum().Should().Be(expected);
  }

  private static List<long> GetInput(string inputFile)
  {
    return P.Long.ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}
