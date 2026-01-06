
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day02
{
  [Theory]
  [InlineData("Day02.Sample.txt", 1227775554)]
  [InlineData("Day02.txt", 31839939622)]
  public void Part1(string inputFile, long expected)
  {
    var data = P.Format("{}-{}", P.Long, P.Long).Star(",").Parse(AdventOfCode2025Loader.ReadLines(inputFile).Single());

    data.SelectMany(it =>
    {
      return Deranged(it.First, it.Second);
    }).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day02.Sample.txt", 4174379265)]
  [InlineData("Day02.txt", 41662374059)] 
  public void Part2(string inputFile, long expected)
  {
    var data = P.Format("{}-{}", P.Long, P.Long).Star(",").Parse(AdventOfCode2025Loader.ReadLines(inputFile).Single());

    data.SelectMany(it =>
    {
      return Deranged2(it.First, it.Second);
    }).Distinct().Sum().Should().Be(expected);
  }

  private IEnumerable<long> Deranged(long first, long second)
  {
    var s = first.ToString();
    var s2 = s[..(s.Length / 2)];
    if (s2 == "") s2 = "1";
    var n = long.Parse(s2);
    var x = long.Parse($"{n}{n}");
    while (x <= second)
    {
      if (x >= first) yield return x;
      n += 1;
      x = long.Parse($"{n}{n}");
    }
  }

  private IEnumerable<long> Deranged2(long first, long second)
  {
    HashSet<long> closed = [];
    for (var current = first; current <= second; current++)
    {
      var currentAsString = $"{current}";
      for (int substringLength = 1; substringLength <= currentAsString.Length / 2; substringLength++)
      {
        if (currentAsString.Length % substringLength == 0 && currentAsString.Length / substringLength > 1)
        {
          var f = long.Parse(Enumerable.Repeat(currentAsString[..substringLength], currentAsString.Length / substringLength).Join(""));
          if (first <= f && f <= second)
          {
            if (closed.Add(f)) yield return f;
          }
        }
      }
    }
  }
}