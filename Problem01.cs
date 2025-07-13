
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace Mng.Quest.CSharp;

public class Problem01: Program
{
  [Theory]
  [InlineData("|||+||||", "|||||||")]
  [InlineData("|||||+||||||||", "|||||||||||||")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  [Theory]
  [InlineData("|||+||||", "|||||||")]
  [InlineData("|||||+||||||||", "|||||||||||||")]
  public void Part2(string input, string expected)
  {
    var ok = CreateState();
    Init.On(Bar, r => r.Write(Blank).Then(ok));
    ok.On(Bar, r => r.Right());
    ok.On('+', r => r.Write(Bar).Then(Halt));
    Write("Problem01.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | OK _ R
OK | OK | R
OK + HALT | R
";
}