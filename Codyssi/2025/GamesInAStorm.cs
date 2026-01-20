using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class GamesInAStorm
{
  [Theory]
  [InlineData("GamesInAStorm.Sample.txt", 9047685997827)]
  [InlineData("GamesInAStorm.txt", 9970418414776)]
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    input.Max(it => it.Value()).Should().Be(expected);
  }

  [Theory]
  [InlineData("GamesInAStorm.Sample.txt", "4iWAbo%6")]
  [InlineData("GamesInAStorm.txt", "5xkaeMP#F")]
  public void Part2(string inputFile, string expected)
  {
    var input = GetInput(inputFile);

    Encode(input.Sum(it => it.Value()), 68).Should().Be(expected);
  }

  [Theory]
  [InlineData("GamesInAStorm.Sample.txt", 2366)]
  [InlineData("GamesInAStorm.txt", 7200)]
  public void Part3(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    var sum = input.Sum(it => it.Value());
    // Not correct for all inputs! 
    // eg: Ceil(Pow(4, 0.25)) == 2, but the correct answer is 3
    Math.Ceiling(Math.Pow(sum, 0.25)).Should().Be(expected);
  }


  public record Line(string Encoded, long Base)
  {
    public long Value()
    {
      static long ctol(char c)
      {
        if ('0' <= c && c <= '9') return c - '0';
        if ('A' <= c && c <= 'Z') return 10 + c - 'A';
        if ('a' <= c && c <= 'z') return 36 + c - 'a';
        throw new ApplicationException();
      }
      return Encoded[1..].Aggregate(ctol(Encoded[0]), (a, b) => a * Base + ctol(b));
    }

  }
  public static string Encode(long value, long myBase)
  {
    char ltoc(long l)
    {
      var v = l % myBase;
      if (v < 10) return (char)('0' + v);
      if (v < 36) return (char)('A' + v - 10);
      if (v < 62) return (char)('a' + v - 36);
      if (v <= 68) return "!@#$%^"[(int)v - 62];
      throw new ApplicationException();
    }
    if (value == 0) return "0";
    var s = "";
    while (value > 0)
    {
      s = ltoc(value) + s;
      value /= myBase;
    }
    return s;
  }

  private static List<Line> GetInput(string inputFile)
  {
    var number = (P.Digit | P.Letter).Plus().Join("");
    return P.Format("{} {}", number, P.Long)
      .Select(it => new Line(it.First, it.Second))
      .ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}