using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem06 : Program
{
  [Theory]
  [InlineData("|||||-||", "|||")]
  [InlineData("||-||", "")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(new BySteps().Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  // [Theory]
  // [InlineData("|||||-||", "|||")]
  // [InlineData("||-||", "")]
  // public void Part2(string input, string expected)
  // {
  //   var result = new LogicMill(new ByRules().Join("")).RunToHalt(input);
  //   result.Result.Should().Be(expected);
  // }


  class BySteps : Program
  {
    public BySteps()
    {
      var previousUp = Init;
      const int max = 110;

      List<State> erase = [CreateState()];
      for (var e = 1; e < max; e++)
      {
        erase.Add(CreateState()
          .On(Bar, s => s.Left().Then(e == 1 ? Halt : erase[e - 1])));
      }

      for (var up = 1; up < max; up++)
      {
        var state = CreateState($"up_{up}");
        previousUp.On(Bar, s => s.Write(Blank).Then(state));
        previousUp = state;

        var zeroWritten = CreateState($"written_{up}_{0}");
        var previousWritten = zeroWritten;
        for (var written = 1; written <= up; written++)
        {
          var writtenState = CreateState($"written_{up}_{written}");
          previousWritten.On(Bar, s => s.Then(writtenState));
          previousWritten = writtenState;
          if (up - written == written)
            writtenState.On(Blank, s => s.Then(Halt));
          else if (up - written < written)
            writtenState.On(Blank, s => s.Write(Enumerable.Repeat(Bar, written - (up - written)).Join(""), after => after.Then(Halt)));
          else if (up - written > written)
            writtenState.On(Blank, s => s.Left().Then(erase[(up - written) - written]));
        }

        state.On('-', s => s.Write(Blank).Then(zeroWritten));
      }

      Write("Problem06.rules");
    }
  }

  class ByRules : Program
  {
    public ByRules()
    {
      var previousUp = Init;
      const int max = 110;

      List<State> write = [CreateState("Zero")];
      for (var down = 1; down < max; down++)
      {
        write.Add(CreateState($"down{down}")
          .On(Bar, s => s.Write(Blank).Then(down == 1 ? Halt : write[down - 1]))
          .On(Blank, s => s.Write(Enumerable.Repeat(Bar, down).Join(""), after => after.Then(Halt)))
        );
      }

      for (var up = 1; up < max; up++)
      {
        var state = CreateState($"up{up}");
        previousUp.On(Bar, s => s.Write(Blank).Then(state));
        previousUp = state;
        state.On('-', s => s.Write(Blank).Then(write[up]));
      }

      Write("Problem06a.rules");
    }
  }
}