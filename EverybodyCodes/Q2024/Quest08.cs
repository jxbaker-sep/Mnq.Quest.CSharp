using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Mng.Quest.CSharp.Utils;
using System.Collections;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest08
{
  [Theory]
  [InlineData("Quest08.1.Sample.txt", 21)]
  [InlineData("Quest08.1.txt", 8191127)]
  public void Part1(string inputFile, long expected)
  {
    var blocks = GetInput(inputFile);
    var n = (long)Math.Floor(Math.Sqrt(blocks)) + 1;
    var width = 1 + 2 * (n - 1);
    var missing = n * n - blocks;
    (width * missing).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest08.2.Sample.txt", 5, 50, 27)]
  [InlineData("Quest08.2.txt", 1111, 20240000, 123278544)]
  public void Part2(string inputFile, long acolytes, long blocksAvailable, long expected)
  {
    var priests = GetInput(inputFile);

    long layer = 1;
    long width = 1;
    long thickness = 1;
    blocksAvailable -= 1;
    while (blocksAvailable > 0)
    {
      layer += 1;
      width += 2;
      thickness = (thickness * priests) % acolytes;
      blocksAvailable -= width * thickness;
    }

    var missing = Math.Abs(blocksAvailable);
    (width * missing).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest08.3.Sample.txt", 5, 160, 2)]
  [InlineData("Quest08.3.txt", 10, 202400000, 41067)]
  public void Part3(string inputFile, long acolytes, long blocksAvailable, long expected)
  {
    var priests = GetInput(inputFile);

    List<long> thicknesses = [1];
    List<long> heights = [1];
    for (int layer = 2; layer < 100_000_000; layer++)
    {
      long width = 1 + (layer - 1) * 2;
      var thickness = ((thicknesses[^1] * priests) % acolytes) + acolytes;
      thicknesses.Add(thickness);
      for (var i = 0; i < heights.Count; i++) heights[i] += thickness;
      heights.Add(thickness);

      var blocksUsed = thicknesses.WithIndices().Sum(it => it.Value * (1 + it.Index * 2));
      if (blocksUsed < blocksAvailable) continue;

      var spaces = Enumerable.Range(1, layer - 1)
        .Select(column =>
          {
            var height = heights[^(column + 1)];
            return ((priests * width) * height) % acolytes;
          }
        )
        .ToList();

      var used = blocksUsed - spaces[..^1].Sum() * 2 - spaces[^1];
      if (used >= blocksAvailable)
      {
        (used - blocksAvailable).Should().Be(expected);
        return;
      }
    }

    throw new ApplicationException();
  }


  private static long GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).Single();
  }

}
