using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem03 : Program
{
  [Theory]
  [InlineData("0", "1")]
  [InlineData("1", "10")]
  [InlineData("1010", "1011")]
  [InlineData("111", "1000")]
  public void Part1(string input, string expected)
  {
    var was0 = CreateState();
    var was1 = CreateState();
    Init.On('1', r => r.Write(Blank).Then(was1));
    Init.On('0', r => r.Write(Blank).Then(was0));

    was0.On('1', r => r.Write('0').Then(was1));
    was0.On('0', r => r.Write('0').Then(was0));
    was0.On('_', r => r.Write('1').Then(Halt));

    var finalize = CreateState()
      .On("1", r => r.Write('0').Left())
      .On("0", r => r.Write('1').Then(Halt))
      .On("_", r => r.Write('1').Then(Halt));

    was1.On('1', r => r.Write('1').Then(was1));
    was1.On('0', r => r.Write('1').Then(was0));
    was1.On('_', r => r.Write('0').Then(finalize).Left());


    var result = new LogicMill(Join()).RunToHalt(input);

    Write("Problem03.rules");
    result.Result.Should().Be(expected);
  }
}