using FluentAssertions;

namespace Mnq.Quest.CSharp.Codyssi;

public class Day02
{
  [Theory]
  [InlineData("Day02.Sample.txt", 9130674516975)]
  [InlineData("Day02.txt", 13449702152592)]
  public void Part1(string inputFile, long expected)
  {
    
    var data = CodyssiLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).ToList();
    var median = data.Select(it => Resolve(inputFile, it)).OrderBy(it => it).ToList()[data.Count / 2];

    median.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day02.Sample.txt", 1000986169836015)]
  [InlineData("Day02.txt", 2395454897118256203)]
  public void Part2(string inputFile, long expected)
  {
    
    var data = CodyssiLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).ToList();
    var evens = data.Where(long.IsEvenInteger).ToList();
    var estimate = Resolve(inputFile, evens.Sum());

    estimate.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day02.Sample.txt", 5496)]
  [InlineData("Day02.txt", 5415)]
  public void Part3(string inputFile, long expected)
  {
    var data = CodyssiLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).ToList();
    long target = 15000000000000;
    var estimate = data.Select(it => (it, Resolve(inputFile, it))).Where(it => it.Item2 < target).MaxBy(it => it.Item2).it;

    estimate.Should().Be(expected);
  }

  private long Resolve(string inputFile, long value)
  {
    Func<long, long> f1 = (a) => a * a * a * 55 + 495;
    Func<long, long> f2 = (a) => a * a * a * 93 + 435;
    if (inputFile.Contains("Sample")) return f1(value);
    return f2(value);
  }
}