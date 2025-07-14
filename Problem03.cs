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
    Init.Skip("10");
    Init.On(Blank, r => r.Left().Then(s => s
      .On("0", r => r.Write("1").Then(Halt))
      .On("1", r => r.Write("0").Left())
      .On(Blank, r => r.Write("1").Then(Halt)))
    );

    var result = new LogicMill(Join()).RunToHalt(input);

    Write("Problem03.rules");
    result.Result.Should().Be(expected);
  }
}