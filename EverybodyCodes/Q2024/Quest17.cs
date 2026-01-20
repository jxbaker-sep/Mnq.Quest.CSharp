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

    (StarSearch(stars) + stars.Count).Should().Be(expected);
  }

  private static long StarSearch(Stars stars)
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
    HashSet<Point> closed = [];
    HashSet<Point> open = [..stars];

    while (open.Count > 0)
    {
      var (Star1, Star2, Distance) = mds
        .Where(it => open.Contains(it.Star1) || open.Contains(it.Star2))
        .Where(it => closed.Count == 0 || closed.Contains(it.Star1) || closed.Contains(it.Star2))
        .MinBy(it => it.Distance);
      closed.Add(Star1);
      closed.Add(Star2);
      open.Remove(Star1);
      open.Remove(Star2);
      total += Distance;
    }

    return total;
  }

  public record Entry(Stars Open, Stars Closed, long Connections);

  private static Stars GetInput(string inputFile)
  {
    return [.. ECLoader.ReadLines(inputFile).Gridify()
      .Items().Where(it => it.Value != '.').Select(it => it.Point)];
  }

}
