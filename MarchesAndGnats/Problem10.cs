using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem10
{
  private static MyProgram? Program = null;

  [Theory]
  [InlineData("hello+world+how-are-you", "|||")]
  public void Part1(string input, string expected)
  {
    Program ??= new();

    var result = new LogicMill(Program.Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  [Fact]
  public void VeryLongTest()
  {
    Program ??= new();

    var count = 500;
    var input = Enumerable.Repeat("a+", count).Join("") + "a";
    var result = new LogicMill(Program.Join()).RunToHalt(input);
    var expected = Enumerable.Repeat("|", count + 1).Join("");
    result.Result.Should().Be(expected);
    Console.WriteLine(result.Steps);
  }

  [Fact]
  public void ByRules()
  {
    var program = new MyProgramByRules();

    var count = 500;
    var input = Enumerable.Repeat("a+", count).Join("") + "a";
    var result = new LogicMill(program.Join()).RunToHalt(input);
    var expected = Enumerable.Repeat("|", count + 1).Join("");
    result.Result.Should().Be(expected);
  }

  private class MyProgramByRules : Program
  {
    public MyProgramByRules()
    {
      string lexicon = Enumerable.Range('a', 'z' - 'a' + 1).Select(it => (char)it).Join("") + "-äöõü";
      const char newline = '+';

      var reverse = CreateState();
      var terminate = CreateState();
      var writeBar = CreateState().On('a', s => s.Write(Bar).Then(Init));
      Init.On(lexicon, s => s.Write('a'));
      Init.On(Blank, s => s.Left().Then(terminate));
      Init.On(newline, s => s.Write('a').Then(reverse).Left());

      reverse.On($"a", s => s.Left());
      reverse.On(Bar, s => s.Then(writeBar));
      reverse.On(Blank, s => s.Then(writeBar));

      terminate.On('a', s => s.Write(Blank).Left());
      terminate.On(Bar, s => s.Left());
      terminate.On(Blank, s => s.Write(Bar).Then(Halt));

      Write("Problem10a.rules");
    }
  }

  private class MyProgram : Program
  {
    public MyProgram()
    {
      string lexicon = Enumerable.Range('a', 'z' - 'a' + 1).Select(it => (char)it).Join("") + "-äöõü";
      const char plus = '+';

      Init.On(lexicon, r => r.Write(Blank));
      Init.Skip(Bar);
      Init.On(Blank, s => s.Write(Bar).Then(Halt));

      var reverse = CreateState("REVERSE");
      reverse.On(lexicon + Bar + plus, s => s.Left());
      reverse.On(Blank, s => s.Then(Init));

      var previous = Init;
      const int max = 341;
      for (var i = 2; i <= max; i++)
      {
        var r = CreateState($"Found{i}");
        previous.On(plus, s => s.Write(Blank).Then(r));
        previous = r;
        r.On(lexicon, r => r.Write(Blank));
        r.Skip(Bar);
        r.On(Blank, s => s.Write(Enumerable.Repeat(Bar, i).Join(""), after => after.Then(Halt)));
        if (i == max)
        {
          var skipper = CreateState($"Skipper{i}");
          r.On(plus, s => s.Write(Blank).Then(skipper));
          skipper.Skip(lexicon + Bar + plus);
          skipper.On(Blank, s => s.Write(Enumerable.Repeat(Bar, i).Join(""), after => after.Left().Then(reverse)));
        }
      }

      Write("Problem10.rules");
    }
  }
}