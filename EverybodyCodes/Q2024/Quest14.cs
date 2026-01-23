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
    Tree(lines.Single()).Tree.Max(it => it.Z).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest14.2.Sample.txt", 32)]
  [InlineData("Quest14.2.txt", 5074)]
  public void Part2(string inputFile, int expected)
  {
    var lines = GetInput(inputFile);
    HashSet<Point3> tree = [.. lines.SelectMany(it => Tree(it).Tree)];
    tree.Count.Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest14.3.Sample.1.txt", 5)]
  [InlineData("Quest14.3.Sample.2.txt", 46)]
  [InlineData("Quest14.3.txt", 1752)]
  public void Part3(string inputFile, int expected)
  {
    var lines = GetInput(inputFile);
    HashSet<Point3> tree = [.. lines.SelectMany(it => Tree(it).Tree)];
    HashSet<Point3> leaves = [.. lines.Select(it => Tree(it).Leaf)];

    tree.Where(it => it.X == 0 && it.Y == 0)
      .Min(trunk => CountToSegments(tree, trunk, leaves))
      .Should().Be(expected);
  }

  static long CountToSegments(HashSet<Point3> tree, Point3 trunk, HashSet<Point3> leaves)
  {
    Dictionary<Point3, long> closed = [];
    closed[trunk] = 0;
    Queue<Point3> open = [];
    open.Enqueue(trunk);

    while (open.TryDequeue(out var current))
    {
      foreach (var v in new[] { Vector3.Down, Vector3.East, Vector3.Up, Vector3.West, Vector3.North, Vector3.South })
      {
        var neighbor = current + v;
        var d = closed[current] + 1;
        if (!tree.Contains(neighbor)) continue;
        if (closed.GetValueOrDefault(neighbor, long.MaxValue) <= d) continue;
        closed[neighbor] = d;
        open.Enqueue(neighbor);
      }
    }

    return leaves.Sum(leaf => closed[leaf]);
  }

  static (HashSet<Point3> Tree, Point3 Leaf) Tree(List<Step> steps)
  {
    HashSet<Point3> tree = [];
    HashSet<Point3> leaves = [];
    var current = Point3.Zero;
    foreach (var step in steps)
    {
      for (long i = 0; i < step.Scalar; i++)
      {
        current += step.V;
        tree.Add(current);
      }
    }
    return (tree, current);
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
