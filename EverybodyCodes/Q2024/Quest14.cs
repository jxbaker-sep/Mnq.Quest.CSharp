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
    var steps = GetInput(inputFile);

    var current = Point3.Zero;
    long highWaterMark = 0;
    foreach(var step in steps)
    {
      current += step.V * step.Scalar;
      highWaterMark = Math.Max(highWaterMark, current.Z);
    }

    highWaterMark.Should().Be(expected);
  }

  public record Step(Vector3 V, long Scalar);

  private static List<Step> GetInput(string inputFile)
  {
    List<Step> result = [];
    var temp = ECLoader.ReadLines(inputFile).Single();
    foreach (var step in temp.Split(","))
    {
      result.Add(new(step[0] switch
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
    return result;
  }

}
