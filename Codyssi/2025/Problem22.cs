using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Numerics;
using System.Data;
using System.ComponentModel;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem22
{
  [Theory]
  [InlineData("Problem22.Sample.1.txt", 3, 3, 5, 146)]
  [InlineData("Problem22.Sample.2.txt", 10, 15, 60, 32545)]
  [InlineData("Problem22.txt", 10, 15, 60, 32422)]
  public void Part1(string inputFile, long maxX, long maxY, long maxZ, int expected)
  {
    var input = GetInput(inputFile);
    input.SelectMany(it => Find(it, [maxX, maxY, maxZ])).Should().HaveCount(expected);
  }

  [Theory]
  [InlineData("Problem22.Sample.1.txt", 3, 3, 5, 23)]
  [InlineData("Problem22.Sample.2.txt", 10, 15, 60, 217)]
  [InlineData("Problem22.txt", 10, 15, 60, 206)]
  public void Part2(string inputFile, long maxX, long maxY, long maxZ, int expected)
  {
    var input = GetInput(inputFile);
    FindPath(input, maxX, maxY, maxZ, 1).Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem22.Sample.1.txt", 3, 3, 5, 8)]
  [InlineData("Problem22.Sample.2.txt", 10, 15, 60, 166)]
  [InlineData("Problem22.txt", 10, 15, 60, 170)]
  public void Part3(string inputFile, long maxX, long maxY, long maxZ, int expected)
  {
    var input = GetInput(inputFile);
    FindPath(input, maxX, maxY, maxZ, 4).Should().Be(expected);
  }

  private static long FindPath(List<Rule> input, long maxX, long maxY, long maxZ, int MaxHits)
  {
    var debris = input.SelectMany(it => Find(it, [maxX, maxY, maxZ])).ToList();

    List<List<Debris>> debrisAtTimestamp = [];
    List<Dictionary<Point4, int>> occupiedAtTimestamp = [];
    debrisAtTimestamp.Add([.. debris]);
    occupiedAtTimestamp.Add(debris.Select(it => it.Position).GroupToCounts());

    Point4 goal = new(maxX - 1, maxY - 1, maxZ - 1, 0);
    
    PriorityQueue<(Point4 Position, int Seconds, int Hits)> open = new(a => a.Seconds + goal.ManhattanDistance(a.Position));

    open.Enqueue((Point4.Zero, 0, 0));
    List<long> limits = [maxX, maxY, maxZ];
    Dictionary<(Point4 Position, int Seconds), int> closed = [];
    closed[(Point4.Zero, 0)] = 0;

    while (open.TryDequeue(out var current))
    {
      if (closed[(current.Position, current.Seconds)] < current.Hits) continue;
      var nextTimestamp = current.Seconds + 1;
      if (nextTimestamp >= debrisAtTimestamp.Count)
      {
        debrisAtTimestamp.Add(debrisAtTimestamp[^1].Select(a => a.Move(limits)).ToList());
        occupiedAtTimestamp.Add(debrisAtTimestamp[^1].Select(it => it.Position).GroupToCounts());
      }
      foreach(var (next, nextHits) in Neighbors(current.Position, occupiedAtTimestamp[nextTimestamp], limits))
      {
        if (current.Hits + nextHits >= MaxHits) continue;
        if (next == goal) return nextTimestamp;
        if (closed.TryGetValue((next, nextTimestamp), out var previous) && previous <= current.Hits + nextHits) continue;
        closed[(next, nextTimestamp)] = current.Hits + nextHits;
        open.Enqueue((next, nextTimestamp, current.Hits + nextHits));
      }
    }

    throw new ApplicationException();
  }

  static IEnumerable<(Point4, int)> Neighbors(Point4 position, Dictionary<Point4, int> debris, List<long> limits)
  {
    foreach(var delta in new[]{ 
      new Point4(1,0,0,0),
      new Point4(-1,0,0,0),
      new Point4(0,1,0,0),
      new Point4(0,-1,0,0),
      new Point4(0,0,1,0),
      new Point4(0,0,-1,0),
      new Point4(0,0,0,0),
    })
    {
      var next = position + delta;
      if (next.X < 0 || next.X >= limits[0]) continue;
      if (next.Y < 0 || next.Y >= limits[1]) continue;
      if (next.Z < 0 || next.Z >= limits[2]) continue;
      if (next == Point4.Zero) {
        yield return (next, 0);
      }
      yield return (next, debris.GetValueOrDefault(next));
    }
  }

  public static IEnumerable<Debris> Find(Rule r, List<long> maxXYZ)
  {
    for (long x = 0; x < maxXYZ[0]; x += 1)
    for (long y = 0; y < maxXYZ[1]; y += 1)
    for (long z = 0; z < maxXYZ[2]; z += 1)
    for (long w = -1; w <= 1; w += 1)
    if (MathUtils.MathMod(x * r.Coefficients.X + y * r.Coefficients.Y + z * r.Coefficients.Z + w * r.Coefficients.W,
      r.Divisor) == r.Remainder) yield return new(new (x, y, z, w), r.Velocity);
  }

  public record Point4(long X, long Y, long Z, long W)
  {
    public long ManhattanDistance(Point4 other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z) + Math.Abs(W - other.W);
    public static Point4 operator +(Point4 lhs, Point4 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
    public static Point4 Zero { get; } = new(0, 0, 0, 0);
  }

  public record Debris(Point4 Position, Point4 Velocity)
  {
    internal Debris Move(List<long> limits)
    {
      var position = new Point4(
        MathUtils.MathMod(Position.X + Velocity.X, limits[0]),
        MathUtils.MathMod(Position.Y + Velocity.Y, limits[1]),
        MathUtils.MathMod(Position.Z + Velocity.Z, limits[2]),
        MathUtils.MathMod(Position.W + Velocity.W + 1, 3) - 1
      );
      return this with { Position =  position};
    }
  }

  public record Rule(Point4 Coefficients, long Divisor, long Remainder, Point4 Velocity);

  private static List<Rule> GetInput(string inputFile)
  {
    var p1 = P.Format("{}x+{}y+{}z+{}a", P.Long, P.Long, P.Long, P.Long)
      .Select(it => new Point4(it.First, it.Second, it.Third, it.Fourth));
    return P.Format("RULE {}: {} DIVIDE {} HAS REMAINDER {} | DEBRIS VELOCITY ({})",
      P.Long, p1, P.Long, P.Long, P.Long.Star(","))
      .Select(it => new Rule(it.Second, it.Third, it.Fourth, new Point4(it.Fifth[0], it.Fifth[1], it.Fifth[2], it.Fifth[3])))
      .ParseMany(CodyssiLoader.ReadLines(inputFile));

  }
}
