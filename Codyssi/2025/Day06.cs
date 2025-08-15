using FluentAssertions;
using Parser;
namespace Mnq.Quest.CSharp.Codyssi;

public class Day06
{
  [Theory]
  [InlineData("Day06.Sample.txt", 59)]
  [InlineData("Day06.txt", 1340)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    input.Where(char.IsLetter).Should().HaveCount(expected);
  }

  [Theory]
  [InlineData("Day06.Sample.txt", 1742)]
  [InlineData("Day06.txt", 35340)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    input.Where(char.IsLetter)
      .Select(ValueOf)
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day06.Sample.txt", 2708)]
  [InlineData("Day06.txt", 51642)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    var corrected =
    input
      .Aggregate("", (a, b) =>
      {
        if (char.IsLetter(b)) return a + b;
        var previous = ValueOf(a[^1]) * 2 - 5;
        while (previous < 1) previous += 52;
        while (previous > 52) previous -= 52;
        return a + ReverseValue(previous);
      });
    corrected.Select(c => ValueOf(c))
      .Sum().Should().Be(expected);
      
  }

  private static int ValueOf(char c)
  {
    return char.IsAsciiLetterLower(c) ? (c - 'a' + 1) : (c - 'A' + 27);
  }

  private static char ReverseValue(int c)
  {
    if (c <= 26) return (char)('a' + (c - 1));
    return (char)('A' + (c - 27));
  }

  private static string GetInput(string inputFile)
  {
    return CodyssiLoader.ReadLines(inputFile).Single();
  }
}