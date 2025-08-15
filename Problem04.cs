using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem04
{
  private static Problem04Program? MyProgram = null;

  [Theory]
  [InlineData("||*|||", "||||||")]
  // [InlineData("|*|", "|")]
  // [InlineData("|*||", "||")]
  // [InlineData("||*|", "||")]
  // [InlineData("||*||", "||||")]
  // [InlineData("|||||*||||||", "||||||||||||||||||||||||||||||")]
  public void Part1(string input, string expected)
  {
    if (MyProgram == null) {
      MyProgram = new();
      // MyProgram.Optimize();
    }
    var result = new LogicMill(MyProgram.Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }
}

public class Problem04Program : Program
{
  public Problem04Program()
  {
    const int n = 20;
    List<State> numbers = [CreateState()];
    List<State> backErase = [CreateState()];
    for (var i = 1; i < n * n; i++)
    {
      // numbers.Add(CreateState($"number{i}").On(Blank, r => r.Write('|').Then(i == 1 ? Halt : numbers[i-1])));
      numbers.Add(CreateState($"number{i}").On(Blank, r => r.Write(Enumerable.Repeat(Bar, i).Join(), after => after.Then(Halt))));
    }

    for (var i = 1; i < n; i++)
    {
      backErase.Add(CreateState($"backErase{i}").On("|", r => r.Write(Blank).Left().Then(i == 1 ? Halt : backErase[i - 1])));
    }

    var previousLhs = Init;

    for (var i = 1; i <= n; i++)
    {
      var lhs = CreateState($"lhs{i}");
      previousLhs.On("|", r => r.Then(lhs));
      lhs.On("*", r =>
      {
        var rhsZero = CreateState($"rhs_{i}_0");
        var previousRhs = rhsZero;
        for (var j = 1; j <= n; j++)
        {
          var rhs = CreateState($"rhs_{i}_{j}");
          previousRhs.On("|", r => r.Then(rhs));
          rhs.On(Blank, r =>
          {
            var goal = i * j;
            var written = i + j + 1; // +1 because we turned the * into a |
            if (written > goal) return r.Left().Then(backErase[written - goal]);
            if (goal <= written) return r.Then(Halt);
            if (goal == written + 1) return r.Write('|').Then(Halt);
            return r.Write('|').Then(numbers[goal - written - 1]);
          });

          previousRhs = rhs;
        }

        return r.Write('|').Then(rhsZero);
      });
      previousLhs = lhs;
    }

    Write("Problem04.rules");
  }
}