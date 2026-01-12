using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest14
{
  [Theory]
  [InlineData("Quest14.1.Sample.txt", 7)]
  [InlineData("Quest14.1.txt", 144)]
  public void Part1(string inputFile, long expected)
  {
    var lines = GetInput(inputFile);
    Tree(lines.Single()).Max(it => it.Z).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest14.2.Sample.txt", 32)]
  [InlineData("Quest14.2.txt", 5074)]
  public void Part2(string inputFile, int expected)
  {
    var lines = GetInput(inputFile);
    Dictionary<Point3, long> overlaps = [];
    foreach(var steps in lines)
    {
      foreach(var p in Tree(steps))
      {
        overlaps[p] = overlaps.GetValueOrDefault(p) + 1;
      }
    }
    overlaps.Count.Should().Be(expected);
  }

  HashSet<Point3> Tree(List<Step> steps)
  {
    HashSet<Point3> result = [];
    var current = Point3.Zero;
    foreach(var step in steps)
    {
      for(long i = 0; i < step.Scalar; i++)
      {
        current += step.V;
        result.Add(current);
      }
    }
    return result;
  }

  public record Step(Vector3 V, long Scalar);

  private static List<List<Step>> GetInput(string inputFile)
  {
    List<List<Step>> result = [];
    var lines = ECLoader.ReadLines(inputFile);
    foreach (var line in lines)
    {
      List<Step> temp = [];
      result.Add(temp);
      foreach (var step in line.Split(","))
      {
        temp.Add(new(step[0] switch
        {
          'U' => Vector3.Up,
          'D' => Vector3.Down,
          'R' => Vector3.East,
          'L' => Vector3.West,
          'F' => Vector3.North,
          'B' => Vector3.South,
          _ => throw new ApplicationException()
        }, Convert.ToInt64(step[1..])));
      }
    }
    return result;
  }

}
