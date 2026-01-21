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

    (StarSearch(stars, long.MaxValue).First() + stars.Count).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest17.3.Sample.txt", 15624)]
  // [InlineData("Quest17.3.txt", 0)]
  public void Part3(string inputFile, long expected)
  {
    var stars = GetInput(inputFile);

    StarSearch(stars, 6).OrderByDescending(it => it).Take(3).Product().Should().Be(expected);
  }

  private static IEnumerable<long> StarSearch(Stars stars, long maxDistance)
  {
    List<(Point Star1, Point Star2, long Distance)> mds = [];
    for (var i = 0; i < stars.Count - 1; i++)
    {
      for (var j = i + 1; j < stars.Count; j++)
      {
        mds.Add((stars[i], stars[j], stars[i].ManhattanDistance(stars[j])));
      }
    }

    long total = 0;
    HashSet<Point> constellation = [];
    HashSet<Point> available = [..stars];

    while (available.Count > 0)
    {
      var (Star1, Star2, Distance) = mds
        .Where(it => available.Contains(it.Star1) || available.Contains(it.Star2))
        .Where(it => constellation.Count == 0 || constellation.Contains(it.Star1) || constellation.Contains(it.Star2))
        .MinBy(it => it.Distance);

      if (Distance >= maxDistance || constellation.Any(it => it.ManhattanDistance(Star1) >= maxDistance || it.ManhattanDistance(Star2) >= maxDistance))
      {
        yield return total;
        total = 0;
        constellation.Clear();
      }
      
      constellation.Add(Star1);
      constellation.Add(Star2);
      available.Remove(Star1);
      available.Remove(Star2);
      total += Distance;
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
