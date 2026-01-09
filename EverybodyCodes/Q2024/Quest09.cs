using FluentAssertions;
using Mnq.Quest.CSharp.EverybodyCodes;

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
  [InlineData("Quest09.1.Sample.txt", 0, 10)]
  [InlineData("Quest09.1.txt", 0, 13603)]
  [InlineData("Quest09.2.Sample.txt", 1, 10)]
  [InlineData("Quest09.2.txt", 1, 4987)]
  public void Part1ViaWalking(string inputFile, int whichStamp, long expected)
  {
    var stamps = StampsCollection[whichStamp];
    var brightnesses = GetInput(inputFile);

    var walking = ComputeWalking(stamps, (int)brightnesses.Max());

    brightnesses.Sum(b =>
    {
      return walking[(int)b];
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
      return Enumerable.Range(0, 51).Select(delta => ComputeMinimumBeetles(half + n - delta, stamps) + ComputeMinimumBeetles(half + delta, stamps)).Min();
    }).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest09.3.Sample.txt", 2, 10449)]
  [InlineData("Quest09.3.txt", 2, 149448)]
  public void Part3ViaWalking(string inputFile, int whichStamp, long expected)
  {
    var stamps = StampsCollection[whichStamp];
    var brightnesses = GetInput(inputFile);

    var walking = ComputeWalking(stamps, (int)brightnesses.Max());

    brightnesses.Sum(b =>
    {
      var half = b / 2;
      var n = b % 2;
      return Enumerable.Range(0, 51).Select(delta => walking[(int)half + (int)n - delta] + walking[(int)half + delta]).Min();
    }).Should().Be(expected);
  }

  readonly Dictionary<long, long> Cache = [];
  public long ComputeMinimumBeetles(long brightness, IReadOnlyList<long> stamps)
  {
    if (brightness == 0) return 0;
    var key = brightness;
    if (Cache.TryGetValue(key, out var previous)) return previous;

    // max number is using the "1" stamp on brightness number of beetles.
    long found = brightness;
    foreach (var stamp in stamps)
    {
      if (stamp == 1) continue; // baked into found above
      if (stamp > brightness) continue;
      var recursive = ComputeMinimumBeetles(brightness - stamp, stamps);
      if (recursive + 1 < found)
      {
        found = recursive + 1;
      }
    }

    Cache[key] = found;
    return found;
  }

  static List<long> ComputeWalking(IReadOnlyList<long> stamps, int maxValue)
  {
    List<long> result = new(maxValue + 1);
    foreach (var i in Enumerable.Range(0, maxValue + 1)) result.Add(i);

    Queue<int> open = new([0]);

    while (open.TryDequeue(out var current))
    {
      var beetles = result[current];
      foreach (var stamp in stamps)
      {
        int v = current + (int)stamp;
        if (v > maxValue) continue;
        if (result[v] > beetles + 1)
        {
          result[v] = beetles + 1;
          open.Enqueue(v);
        }
      }
    }

    return result;
  }

  private static List<long> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).ToList();
  }

}
