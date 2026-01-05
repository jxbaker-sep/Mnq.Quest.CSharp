using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest07
{
  [Theory]
  [InlineData("Quest07.1.Sample.txt", "BDCA")]
  [InlineData("Quest07.1.txt", "ICKAJHFBG")]
  public void Part1(string inputFile, string expected)
  {
    var plans = GetInput(inputFile);

    plans.OrderByDescending(plan => Gather(plan, 10)).Select(it => it[0]).Join("").Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest07.2.Sample.txt", "Quest07.2.Track.Sample.txt", 1, "DCBA")]
  [InlineData("Quest07.2.Sample.txt", "Quest07.2.Track.Sample.txt", 10, "DCBA")]
  [InlineData("Quest07.2.txt", "Quest07.2.Track.txt", 10, "IACKFGDBJ")]
  public void Part2(string inputFile, string trackFile, int loops, string expected)
  {
    var plans = GetInput(inputFile);
    var temp = GetTrack(trackFile);

    plans.OrderByDescending(plan => GatherOnTrack(plan, temp, loops)).Select(it => it[0]).Join("").Should().Be(expected);
  }

  static long GatherOnTrack(List<string> plan, string track, int loops)
  {
    var current = 10;
    var total = 0;
    var currentLoop = 0;
    var trackIndex = 1;
    var step = 0;
    plan = plan[1..];
    while (currentLoop < loops)
    {
      var currentAction = plan[step++ % plan.Count];
      switch (track[trackIndex++ % track.Length])
      {
        case 'S':
          currentLoop += 1;
          break;
        case '=':
          break;
        case '+':
          currentAction = "+";
          break;
        case '-':
          currentAction = "-";
          break;
        default: throw new ApplicationException();
      }

      switch (currentAction)
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

  private static string GetTrack(string inputFile)
  {
    string s1 = "";
    string s2 = "";
    foreach (var line in ECLoader.ReadLines(inputFile))
    {
      if (line[1] != ' ')
      {
        if (line[0] == 'S') s1 = line;
        else s2 += line;
      }
      else
      {
        s2 += line[0];
        s1 += line[^1];
      }
    }
    return s1 + s2.Reverse().Join();
  }
}
