using FluentAssertions;
using Parser;
using Utils;
namespace Mnq.Quest.CSharp.Codyssi;

public class Day04
{
  [Theory]
  [InlineData("Day04.Sample.txt", 1247)]
  [InlineData("Day04.txt", 133984)]
  public void Part1(string inputFile, long expected)
  {
    var map = Enumerable.Range('A', 'Z' - 'A' + 1).Select(it => (char)it).ToDictionary(it => it, it => (long)(it - 'A' + 1));
    var lines = GetRanges(inputFile);

    lines.Select(line => line.Sum(it => map[it])).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day04.Sample.txt", 219)]
  [InlineData("Day04.txt", 26598)]
  public void Part2(string inputFile, long expected)
  {
    var map = Enumerable.Range('A', 'Z' - 'A' + 1).Select(it => (char)it).ToDictionary(it => it, it => (long)(it - 'A' + 1));
    for (var i = 0; i < 10; i++) map[(char)('0' + i)] = i;
    var lines = GetRanges(inputFile).Select(line => {
      var l = line.Length;
      var s = l / 10;
      return line[0..s] + $"{l - 2 * s}" + line[^s..];
    });

    lines.Select(line => line.Sum(it => map[it])).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day04.Sample.txt", 539)]
  [InlineData("Day04.txt", 45778)]
  public void Part3(string inputFile, long expected)
  {
    var map = Enumerable.Range('A', 'Z' - 'A' + 1).Select(it => (char)it).ToDictionary(it => it, it => (long)(it - 'A' + 1));
    for (var i = 0; i < 10; i++) map[(char)('0' + i)] = i;
    var lines = GetRanges(inputFile).Select(line => {
      var temp = line[1..].Aggregate(new List<(int, char)> { (1, line[0]) }, (a, b) => { 
        if (b == a[^1].Item2) a[^1] = (a[^1].Item1 + 1, b);
        else {
          a.Add((1, b));
        }
        return a;
      });
      return temp.Select(it => $"{it.Item1}{it.Item2}").Join("");
    }).ToList();

    lines.Select(line => line.Sum(it => map[it])).Sum().Should().Be(expected);
  }


  private static List<string> GetRanges(string inputFile)
  {
    return CodyssiLoader.ReadLines(inputFile).ToList();
  }
}