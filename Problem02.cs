using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem02 : Program
{
  [Theory]
  [InlineData("|||||||", "O")]
  [InlineData("||||||", "E")]
  public void Part1(string input, string expected)
  {
    var odd = CreateState();
    var even = CreateState();

    Init.On(Bar, r => r.Write(Blank).Then(odd));
    odd.On(Bar, r => r.Write(Blank).Then(even));
    odd.On(Blank, r => r.Write('O').Then(Halt));
    even.On(Bar, r => r.Write(Blank).Then(odd));
    even.On(Blank, r => r.Write('E').Then(Halt));

    Write("Problem02.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }
}