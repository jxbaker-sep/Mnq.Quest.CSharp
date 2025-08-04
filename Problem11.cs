using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem11 : Program
{
  static int count1 = 0;
  static int count2 = 0;

  [Theory]
  [InlineData("0", "1")]
  [InlineData("1", "2")]
  [InlineData("9", "10")]
  [InlineData("14", "15")]
  [InlineData("19", "20")]
  [InlineData("919", "920")]
  [InlineData("91912", "91913")]
  [InlineData("990", "991")]
  [InlineData("998", "999")]
  [InlineData("999", "1000")]
  public void ByRules(string input, string expected)
  {
    var z9 = "0123456789";

    var reverse = CreateState();

    for (var i = 0; i <= 8; i++)
    {
      reverse.On($"{i}", r => r.Write($"{i + 1}"[0]).Then(Halt));
    }
    reverse.On('9', r => r.Write('0').Left());
    reverse.On(Blank, r => r.Write('1').Then(Halt));

    Init.Skip(z9);
    Init.On(Blank, r => r.Left().Then(reverse));

    Write("Problem11.ByRules.rules");


    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
    count1 += result.Steps;
    Console.WriteLine($"{count1} {count2}");
  }

  [Theory]
  [InlineData("0", "1")]
  [InlineData("1", "2")]
  [InlineData("9", "10")]
  [InlineData("14", "15")]
  [InlineData("19", "20")]
  [InlineData("919", "920")]
  [InlineData("91912", "91913")]
  [InlineData("990", "991")]
  [InlineData("998", "999")]
  [InlineData("999", "1000")]
  public void BySteps(string input, string expected)
  {
    List<State> carry = Enumerable.Repeat(0, 10).Select(_ => CreateState()).ToList();
    for (var c = 0; c < 10; c++)
    {
      if (c == 0) {
        Init.On('0', r => r.Write('1').Then(Halt));
      } else {
        Init.On($"{c}", r => r.Write(Blank).Then(carry[c]));
      }
      var me = carry[c];
      for (var next = 0; next < 10; next++)
      {
        me.On($"{next}", r => r.Write($"{c}"[0]).Then(carry[next]));
      }
      if (c < 9)
      {
        me.On(Blank, r => r.Write($"{c + 1}"[0]).Then(Halt));
      }
      else
      {
        var reverse = CreateState();

        for (var k = 0; k <= 8; k++)
        {
          reverse.On($"{k}", r => r.Write($"{k + 1}"[0]).Then(Halt));
        }
        reverse.On('9', r => r.Write('0').Left());
        reverse.On(Blank, r => r.Write('1').Then(Halt));
        me.On(Blank, r => r.Write('0').Left().Then(reverse));
      }
    }

    Write("Problem11.BySteps.rules");


    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
    count2 += result.Steps;
    Console.WriteLine($"{count1} {count2}");
  }
}