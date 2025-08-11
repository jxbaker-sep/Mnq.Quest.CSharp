using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem12 : Program
{
  [Theory]
  [InlineData("2+5", "7")]
  [InlineData("37+89", "126")]
  [InlineData("999+1", "1000")]
  [InlineData("1+999", "1000")]
  [InlineData("987+987654", "988641")]
  [InlineData("987654+987", "988641")]
  public void BySteps(string input, string expected)
  {
    const string digits = "0123456789";
    const char plus = '+';
    const char equals = '=';
    const char carryEquals = 'c';

    Init.On(plus, r => r.Write(Blank));
    Init.On(equals, r => r.Write(Blank).Then(Halt));
    Init.On(carryEquals, r => r.Write('1').Then(Halt));
    var lhs = Enumerable.Range(0, 10).Select(i => CreateState($"lhs_{i}")).ToList();
    for (var i = 0; i < 10; i++) Init.OnDigit(i, r => r.Write(Blank).Then(lhs[i]));

    var reverse = CreateState("reverse");
    var reverseCarry = CreateState("reverseCarry");
    var reverse1 = CreateState("reverse1")
      .SkipLeft(digits + plus + equals + carryEquals)
      .On(Blank, Init);
    reverse.On(digits + plus, r => r.Write(equals).Left().Then(reverse1));
    reverse.On(Blank, Halt);
    reverseCarry.On(digits + plus, r => r.Write(carryEquals).Left().Then(reverse1));
    reverseCarry.On(Blank, r => r.Write('1').Then(Halt));

    var incrementThenHalt = CreateState("incrementThenHalt");
    for (var i = 0; i <=8; i++) incrementThenHalt.OnDigit(i, r => r.WriteDigit(i + 1).Then(Halt));
    incrementThenHalt.OnDigit(9, r => r.Write('0').Left());
    incrementThenHalt.On(Blank, r => r.Write('1').Then(Halt));

    for (var i = 0; i < 10; i++)
    {
      lhs[i].On(equals, r => r.WriteDigit(i).Then(Halt));
      lhs[i].On(carryEquals, r => r.WriteDigit(i + 1).Left().Then(i + 1 >= 10 ? incrementThenHalt : Halt));
      for (var j = 0; j < 10; j++) lhs[i].OnDigit(j, r => r.WriteDigit(i).Then(lhs[j]));

      var rhs = Enumerable.Range(0, 10).Select(j => CreateState($"rhs_{i}_{j}")).ToList();
      lhs[i].On(plus, rhs[0]);


      for (var j = 0; j < 10; j++)
      {
        for (var k = 0; k < 10; k++) rhs[j].OnDigit(k, rhs[k]);
        rhs[j].On(Blank, r => r.WriteDigit(i+j).Left().Then(i + j >= 10 ? reverseCarry : reverse));
        rhs[j].On(equals, r => r.WriteDigit(i+j).Left().Then(i + j >= 10 ? reverseCarry : reverse));
        rhs[j].On(carryEquals, r => r.WriteDigit(i+j+1).Left().Then(i + j+1 >= 10 ? reverseCarry : reverse));
      }
    }


    // #######################################################

    Write("Problem12.BySteps.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

}