using System.Globalization;
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
  public void ByRules(string input, string expected)
  {
    var digits = "0123456789";
    var plus = '+';
    var ignore = '.';
    var carry = 'c';
    var equals = '=';

    List<State> lookingLhs = Enumerable.Range(0, 10).Select(i => CreateState($"lhs_{i}")).ToList();

    for (var i = 0; i <= 9; i++)
    {
      char token = $"{i}"[0];
      Init.On(token, r => r.Then(lookingLhs[i]));
      foreach (var other in lookingLhs) other.On(token, r => r.Then(lookingLhs[i]));
    }

    var rewind = CreateState("rewind");
    var finalize = CreateState("finalize");
    var plusZero = CreateState("plusZero");

    for (var i = 0; i <= 9; i++)
    {
      var lhs = lookingLhs[i];
      lhs.Skip(ignore);
      char token = $"{i}"[0];

      List<State> lookingRhs = Enumerable.Range(0, 10).Select(j => CreateState($"rhs_{i}_{j}")).ToList();
      lhs.On(plus, r => r.Then(lookingRhs[0]));

      for (var j = 0; j <= 9; j++)
      {
        var rhs = lookingRhs[j];
        char token2 = $"{j}"[0];
        if (i == 0) plusZero.On(token2, r => r.Then(rhs));
        foreach (var other in lookingRhs) other.On(token2, r => r.Then(rhs));

        var toEnd = CreateState($"toEnd_{i}_{j}");
        rhs.On(Blank, r => r.Write(equals).Then(toEnd));
        rhs.On(equals, r => r.Then(toEnd));
        rhs.Skip(ignore);

        var carry_carry = (i + j + 1) >= 10 ? $"{(i+j+1) % 10}c" : $"{(i+j+1) % 10}";
        var non_carry = (i + j) >= 10 ? $"{(i+j) % 10}c" : $"{(i+j) % 10}";
        toEnd.Skip(digits);
        toEnd.On(carry, r => r.Write(carry_carry, after => after.Left().Then(rewind)));
        toEnd.On(Blank, r => r.Write(non_carry, after => after.Left().Then(rewind)));
      }
    }

    Init.Skip(Blank);
    Init.On(plus, r => r.Write(Blank).Then(plusZero));
    Init.On(ignore, r => r.Write(Blank));
    plusZero.On(ignore, r => r.Write(Blank).Then(finalize));
    finalize.On(ignore, r => r.Write(Blank));
    var reverse = CreateState("reverse");
    finalize.On(equals, r => r.Then(reverse));

    var reverseFF = CreateState("reverseFF");
    reverseFF.Skip(digits);
    reverseFF.On(equals, r => r.Then(reverse));
    reverse.Skip(ignore);
    foreach(var digit in digits + carry) {
      var me = CreateState($"reverse_{digit}");
      reverse.On(digit, r => r.Write(ignore).Left().Then(me));
      me.SkipLeft(digits + carry + equals + ignore);
      me.On(Blank, r => r.Write(digit == carry ? '1' : digit).Then(reverseFF));
    }
    var finalize2 = CreateState("finalize2");
    reverse.LeftOn(Blank, finalize2); // TODO: could finalize after "c" too
    finalize2.On(ignore, r => r.Left().Write(Blank));
    finalize2.On(equals, r => r.Left().Write(Blank).Then(Halt));

    var rewind_delete_1 = CreateState("rewind_delete_1");
    var rewind_delete_2 = CreateState("rewind_delete_2");
    var rewind_delete_3 = CreateState("rewind_delete_3");
    rewind.SkipLeft(digits);
    rewind.On(equals, r => r.Left().Then(rewind_delete_1));
    rewind_delete_1.SkipLeft(ignore);
    rewind_delete_1.On(digits, r => r.Write(ignore).Left().Then(rewind_delete_2));
    rewind_delete_1.On(plus, r => r.Left().Then(rewind_delete_3));
    rewind_delete_2.SkipLeft(digits);
    rewind_delete_2.On(Blank, r => r.Then(plusZero));
    rewind_delete_2.On(plus, r => r.Left().Then(rewind_delete_3));
    rewind_delete_3.SkipLeft(ignore);
    rewind_delete_3.On(Blank, r => r.Then(Init));
    rewind_delete_3.On(digits, r => r.Write(ignore).Left().Then(Init));


    // #######################################################

    Write("Problem12.ByRules.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

}