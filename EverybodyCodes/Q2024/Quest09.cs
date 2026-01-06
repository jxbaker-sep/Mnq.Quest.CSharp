using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Mng.Quest.CSharp.Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest09
{
  IReadOnlyList<IReadOnlyList<long>>  StampsCollection = [
    [10, 5, 3, 1],
    [30, 25, 24, 20, 16, 15, 10, 5, 3, 1],
    [101, 100, 75, 74, 50, 49, 38, 37, 30, 25, 24, 20, 16, 15, 10, 5, 3, 1]
  ];

  [Theory]
  [InlineData("Quest09.1.Sample.txt", 0, 10)]
  [InlineData("Quest09.1.txt", 0, 13603)]
  [InlineData("Quest09.2.Sample.txt", 1, 10)]
  [InlineData("Quest09.2.txt", 1, 4987)]
  public void Part1(string inputFile, int whichStamp, long expected)
  {
    var stamps = StampsCollection[whichStamp];
    var brightnesses = GetInput(inputFile);

    brightnesses.Sum(b =>
    {
      return ComputeMinimumBeetles(b, stamps);
    }).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest09.3.Sample.txt", 2, 10449)]
  // [InlineData("Quest09.3.txt", 2, 0)]
  public void Part3(string inputFile, int whichStamp, long expected)
  {
    var stamps = StampsCollection[whichStamp];
    var brightnesses = GetInput(inputFile);

    brightnesses.Sum(b =>
    {
      var half = b / 2;
      var n = b % 2;
      return Enumerable.Range(0, 51).Min(delta => ComputeMinimumBeetles(half - delta, stamps) + ComputeMinimumBeetles(half + delta + n, stamps));
    }).Should().Be(expected);
  }

  public long ComputeMinimumBeetles(long brightness, IReadOnlyList<long> stamps)
  {
    // maps total brightness to number of stamps used to get to that brightness
    Dictionary<long, long> Closed = [];
    Closed[0] = 0;

    Queue<(long Brightness, long Beetles)> open = new([(0, 0)]);

    while (open.TryDequeue(out var current))
    {
      if (current.Beetles >= Closed.GetValueOrDefault(brightness, long.MaxValue)) continue;
      var nextBeetles = current.Beetles + 1;
      foreach(var stamp in stamps)
      {
        var nextBrightness = current.Brightness + stamp;
        if (nextBrightness > brightness) continue;
        if (Closed.GetValueOrDefault(nextBrightness, long.MaxValue) <= nextBeetles) continue;
        Closed[nextBrightness] = nextBeetles;
        open.Enqueue((nextBrightness, nextBeetles));
      }
    }

    return Closed[brightness];
  }


  private static List<long> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).ToList();
  }

}
