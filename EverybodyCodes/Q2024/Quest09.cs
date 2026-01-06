using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Mng.Quest.CSharp.Utils;
using System.Net.Sockets;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest09
{
  IReadOnlyList<IReadOnlyList<long>> StampsCollection = [
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
  [InlineData("Quest09.3.txt", 2, 149448)]
  public void Part3(string inputFile, int whichStamp, long expected)
  {
    var stamps = StampsCollection[whichStamp];
    var brightnesses = GetInput(inputFile);

    brightnesses.Sum(b =>
    {
      var half = b / 2;
      var n = b % 2;
      if (n == 0)
      {
        // Select then Min to avoid compilation warning
        return Enumerable.Range(0, 51).Select(delta => ComputeMinimumBeetles(half - delta, stamps) + ComputeMinimumBeetles(half + delta, stamps)).Min();
      }
      else
      {
        // We don't need to add the +1 to the second half; 
        // all possibilities are computed here: (+1, +0) to (-49, +50)
        // and this avoids the incorrect sequence (-50, +51),
        // which would be more than 100 units apart.
        return Enumerable.Range(0, 51).Select(delta => ComputeMinimumBeetles(half + 1 - delta, stamps) + ComputeMinimumBeetles(half + delta, stamps)).Min();
      }
    }).Should().Be(expected);
  }

  readonly Dictionary<long, long?> Cache = [];
  public long? ComputeMinimumBeetles(long brightness, IReadOnlyList<long> stamps)
  {
    if (brightness == 0) return 0;
    if (brightness < 0) return null;
    var key = brightness;
    if (Cache.TryGetValue(key, out var previous)) return previous;

    long? found = null;
    foreach (var stamp in stamps)
    {
      var recursive = ComputeMinimumBeetles(brightness - stamp, stamps);
      if (recursive is { } r)
      {
        if (r + 1 < (found ?? long.MaxValue))
        {
          found = r + 1;
        }
      }
    }

    Cache[key] = found;
    return found;
  }

  public long ComputeMinimumBeetles2(long brightness, IReadOnlyList<long> stamps)
  {
    // maps total brightness to number of stamps used to get to that brightness
    Dictionary<long, long> Closed = [];
    Closed[0] = 0;

    Queue<(long Brightness, long Beetles)> open = new([(0, 0)]);

    while (open.TryDequeue(out var current))
    {
      if (current.Beetles >= Closed.GetValueOrDefault(brightness, long.MaxValue)) continue;
      var nextBeetles = current.Beetles + 1;
      foreach (var stamp in stamps)
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
