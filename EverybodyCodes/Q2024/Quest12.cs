using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest12
{
  [Theory]
  [InlineData("Quest12.1.Sample.txt", 13)]
  [InlineData("Quest12.1.txt", 243)]
  [InlineData("Quest12.2.Sample.txt", 22)]
  [InlineData("Quest12.2.txt", 20669)]
  public void Part1(string inputFile, long expected)
  {
    var world = GetInput(inputFile);

    world.Targets.Sum(t => DestroyTarget(t, world.Cannons)).Should().Be(expected);
  }

  private long DestroyTarget(Target target, List<Cannon> cannons)
  {
    foreach(var (cannon, scalar) in cannons)
    {
      var dy = target.P.Y - cannon.Y;
      var d = target.P.X - cannon.X - dy;
      if (d % 3 != 0) continue;
      return (d / 3) * scalar * target.Hardness;
    }
    throw new ApplicationException();
  }

  private record Cannon(Point P, long Scalar);
  private record Target(Point P, long Hardness);
  private record World(List<Cannon> Cannons, List<Target> Targets);
  private static World GetInput(string inputFile)
  {
    List<Cannon> Cannons = [];
    List<Target> Targets = [];
    var grid = ECLoader.ReadLines(inputFile).Gridify();
    Cannons.Add(new (grid.Items().Single(it => it.Value == 'A').Point, 1));
    Cannons.Add(new (grid.Items().Single(it => it.Value == 'B').Point, 2));
    Cannons.Add(new (grid.Items().Single(it => it.Value == 'C').Point, 3));
    Targets.AddRange(grid.Items().Where(it => it.Value == 'T' || it.Value == 'H').Select(it => new Target(it.Point, it.Value == 'H' ? 2 : 1))
      .OrderBy(target => target.P.Y).ThenBy(target => target.P.X));

    return new(Cannons, Targets);
  }

}
