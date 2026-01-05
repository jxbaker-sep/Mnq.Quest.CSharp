using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest07
{
  [Theory]
  [InlineData("Quest07.Sample.1.txt", "BDCA")]
  [InlineData("Quest07.1.txt", "ICKAJHFBG")]
  public void Part1(string inputFile, string expected)
  {
    var plans = GetInput(inputFile);

    plans.OrderByDescending(plan => Gather(plan, 10)).Select(it => it[0]).Join("").Should().Be(expected);
  }

  static long Gather(List<string> plan, int segments)
  {
    var current = 10;
    var total = 0;
    for (int i = 0; i < segments; i++)
    {
      switch (plan[1 + (i % (plan.Count - 1))])
      {
        case "+":
          current += 1;
          break;
        case "-":
          current -= 1;
          break;
      }
      total += current;
    }
    return total;
  }

  private static List<List<string>> GetInput(string inputFile)
  {
    List<List<string>> result = [];
    foreach (var line in ECLoader.ReadLines(inputFile))
    {
      List<string> item = [];
      var temp = line.Split(":");
      item.Add(temp[0]);
      item.AddRange(temp[1].Split(','));
      result.Add(item);
    }
    return result;
  }
}
