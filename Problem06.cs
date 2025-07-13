using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem06
{
  [Theory]
  [InlineData("|||||-||", "|||")]
  [InlineData("||-||", "")]
  public void Part1(string input, string expected)
  {
    List<string> rules = [];
    rules.Add("INIT | S1 _ R");
    const int max = 256;
    for (var a = 1; a < max; a++)
    {
      var lhs = $"S{a}";
      rules.Add($"{lhs} | S{a+1} _ R");
      rules.Add($"{lhs} - Write{a} _ R");
    }
    for (var b = 2; b < max; b++)
    {
      rules.Add($"Write{b} | Write{b - 1} _ R");
      rules.Add($"Write{b} _ Write{b - 1} | R");
    }
    rules.Add($"Write1 | HALT _ R");
    rules.Add($"Write1 _ HALT | R");
    File.WriteAllLines("/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/Problem06.rules", rules);

    var result = new LogicMill(rules.Join("\n")).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

}