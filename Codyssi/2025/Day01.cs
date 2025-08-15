
using FluentAssertions;

namespace Mnq.Quest.CSharp.Codyssi;

public class Day01
{
  [Theory]
  [InlineData("Day01.Sample.txt", 21)]
  [InlineData("Day01.txt", -126)]
  public void Part1(string inputFile, long expected)
  {
    var data = CodyssiLoader.ReadLines(inputFile);
    var magnitudes = data[..^1].Select(line => Convert.ToInt64(line)).ToList();
    var corrections = data[^1];

    var answer = magnitudes[1..].Zip(corrections).Aggregate(magnitudes[0],
      (a, b) => a + (b.First * (b.Second == '-' ? -1 : 1)));

    answer.Should().Be(expected);
  }

    [Theory]
  [InlineData("Day01.Sample.txt", 23)]
  [InlineData("Day01.txt", -106)]
  public void Part2(string inputFile, long expected)
  {
    var data = CodyssiLoader.ReadLines(inputFile);
    var magnitudes = data[..^1].Select(line => Convert.ToInt64(line)).ToList();
    var corrections = data[^1];

    var answer = magnitudes[1..].Zip(corrections.Reverse()).Aggregate(magnitudes[0],
      (a, b) => a + (b.First * (b.Second == '-' ? -1 : 1)));

    answer.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day01.Sample.txt", 189)]
  [InlineData("Day01.txt", -886)]
  public void Part3(string inputFile, long expected)
  {
    var data = CodyssiLoader.ReadLines(inputFile);
    var magnitudes = data[..^1].Select(line => Convert.ToInt64(line))
      .Aggregate(new List<long>{0}, (a, b) =>
      {
        if (a[^1] < 10) a[^1] = a[^1] * 10 + b;
        else a.Add(b);
        return a;
      });
    var corrections = data[^1];

    var answer = magnitudes[1..].Zip(corrections.Reverse()).Aggregate(magnitudes[0],
      (a, b) => a + (b.First * (b.Second == '-' ? -1 : 1)));

    answer.Should().Be(expected);
  }
}