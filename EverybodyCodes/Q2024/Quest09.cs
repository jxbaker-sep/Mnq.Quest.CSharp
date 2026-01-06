using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest09
{
  [Theory]
  [InlineData("Quest09.1.Sample.txt", 21)]
  [InlineData("Quest09.1.txt", 8191127)]
  public void Part1(string inputFile, long expected)
  {
    var blocks = GetInput(inputFile);
    var n = (long)Math.Floor(Math.Sqrt(blocks)) + 1;
    var width = 1 + 2 * (n - 1);
    var missing = n * n - blocks;
    (width * missing).Should().Be(expected);
  }


  private static long GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).Single();
  }

}
