using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using System.Data;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem03
{
  [Theory]
  [InlineData("Problem03.Sample.txt", 78)]
  [InlineData("Problem03.txt", 7184)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    input.Sum(it => it.Base).Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem03.Sample.txt", 3487996082)]
  [InlineData("Problem03.txt", 401619711441)]
  public void Part2(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    input.Select(it => Convert.ToInt64(it.Reading, it.Base)).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem03.Sample.txt", "30PzDC")]
  [InlineData("Problem03.txt", "5L8xYuQ")]
  public void Part3(string inputFile, string expected)
  {
    var input = GetInput(inputFile);
    ToBase65(input.Select(it => Convert.ToInt64(it.Reading, it.Base)).Sum()).Should().Be(expected);
  }

  private static string ToBase65(long value)
  {
    string map = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#";
    string rep = "";
    for (; value > 0; value /= 65)
    {
      int d = (int)(value % 65);
      rep = map[d] + rep;
    }
    return rep;
  }

  public record ReadingFormat(string Reading, int Base);

  private static List<ReadingFormat> GetInput(string inputFile)
  {
    return P.Format("{} {}", (P.Letter | P.Digit).Star().Join(), P.Int)
      .Select(it => new ReadingFormat(it.First, it.Second))
      .ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}
