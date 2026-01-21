using System.Diagnostics;
using FluentAssertions;
using MathNet.Numerics;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

using Stars = IReadOnlyList<Point>;

public class Quest17
{

  [Theory]
  [InlineData("Quest17.1.Sample.txt", 16)]
  [InlineData("Quest17.1.txt", 136)]
  [InlineData("Quest17.2.txt", 1209)]
  public void Part1(string inputFile, long expected)
  {
    var stars = GetInput(inputFile);

    StarSearch(stars, long.MaxValue).First().Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest17.3.Sample.txt", 15624)]
  [InlineData("Quest17.3.txt", 4260330828)] // ~1315ms
  public void Part3(string inputFile, long expected)
  {
    Stopwatch sw = new();
    sw.Start();
    var stars = GetInput(inputFile);

    var temp = StarSearch(stars, 6).ToList();

    StarSearch(stars, 6).OrderByDescending(it => it).Take(3).Product().Should().Be(expected);
    Console.WriteLine(sw.ElapsedMilliseconds);
  }

  private static IEnumerable<long> StarSearch(Stars stars, long maxDistance)
  {
    List<(Point Star1, Point Star2, long Distance)> mdsTemp = [];
    for (var i = 0; i < stars.Count - 1; i++)
    {
      for (var j = i + 1; j < stars.Count; j++)
      {
        mdsTemp.Add((stars[i], stars[j], stars[i].ManhattanDistance(stars[j])));
      }
    }

    mdsTemp.Sort((a, b) => a.Distance.CompareTo(b.Distance));

    LinkedList<(Point Star1, Point Star2, long Distance)> ll = new(mdsTemp);

    long total = 0;
    HashSet<Point> available = [..stars];

    while (available.Count > 0)
    {
      (Point Star1, Point Star2, long Distance) found;
      if (total == 0)
      {
        var node = ll.Nodes()
                   .First(it => available.Contains(it.Value.Star1) && available.Contains(it.Value.Star2));
        if (node.Value.Distance >= maxDistance)
        {
          // items in available are all >= MaxDistance apart; we could return single-star constellations
          // but we don't need to for this problem.
          yield break;
        }
        ll.Remove(node);
        found = node.Value;
        total += 2;
      }
      else
      {
        var nodes = ll.Nodes()
                   .Where(it => available.Contains(it.Value.Star1) != available.Contains(it.Value.Star2))
                   .TakeWhile(it => it.Value.Distance < maxDistance)
                   .Take(1).ToList();
        if (nodes.Count == 0)
        {
          yield return total;
          total = 0;
          continue;
        }
        ll.Remove(nodes[0]);
        found = nodes[0].Value;
        total += 1;
      }

      available.Remove(found.Star1);
      available.Remove(found.Star2);
      total += found.Distance;
    }

    if (total > 0) yield return total;
  }


  public record Entry(Stars Open, Stars Closed, long Connections);

  private static Stars GetInput(string inputFile)
  {
    return [.. ECLoader.ReadLines(inputFile).Gridify()
      .Items().Where(it => it.Value != '.').Select(it => it.Point)];
  }

}
