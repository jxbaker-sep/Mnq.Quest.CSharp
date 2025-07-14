using System.Runtime.ExceptionServices;
using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem04 : Program
{
  [Theory]
  [InlineData("||*|||", "||||||")]
  [InlineData("|*|", "|")]
  [InlineData("|*||", "||")]
  [InlineData("||*|", "||")]
  [InlineData("||*||", "||||")]
  [InlineData("|||||*||||||", "||||||||||||||||||||||||||||||")]
  public void Part1(string input, string expected)
  {
    const int n = 20;
    List<State> numbers = [CreateState()];
    List<State> backErase = [CreateState()];
    for (var i = 1; i < n * n; i++)
    {
      numbers.Add(CreateState($"number{i}").On(Blank, r => r.Write('|').Then(i == 1 ? Halt : numbers[i-1])));
      backErase.Add(CreateState($"backErase{i}")).On("|", r => r.Write(' ')).)
    }

    var previousLhs = Init;

    for (var i = 1; i <= n; i++) {
      var lhs = CreateState($"lhs{i}");
      previousLhs.On("|", r => r.Then(lhs));
      lhs.On("*", r => {
        if (i == 1) return r.Write(Blank).Then(Halt);
        var rhsZero = CreateState($"rhs_{i}_0");
        var previousRhs = rhsZero;
        for (var j = 1; j <= n; j++)
        {
          var rhs = CreateState($"rhs_{i}_{j}");
          previousRhs.On("|", r => r.Then(rhs));
          rhs.On(Blank, r => {
            var goal = i * j;
            var written = i + j + 1; // +1 because we turned the * into a |
            if (written > goal) return r.Write(Blank).Then(Halt);
            if (goal <= written) return r.Then(Halt);
            if (goal == written + 1) return r.Write('|').Then(Halt);
            return r.Write("|").Then(numbers[goal - written - 1]);
          });
          
          previousRhs = rhs;
        }

        return r.Write('|').Then(rhsZero);
      });
      previousLhs = lhs;
    }

    Write("Problem04.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }
}