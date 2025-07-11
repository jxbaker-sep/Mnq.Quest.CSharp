using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem03
{
  [Theory]
  [InlineData("0", "1")]
  [InlineData("1", "10")]
  [InlineData("1010", "1011")]
  [InlineData("111", "1000")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT 0 LZ 0 R
INIT 1 INIT 1 R
INIT _ INCREMENT _ L

LZ 0 LZ 0 R
LZ 1 INIT 1 R
LZ _ END1 _ L
END1 0 HALT 1 R

INCREMENT 0 HALT 1 L
INCREMENT 1 INCREMENT 0 L
INCREMENT _ HALT 1 L
";
}