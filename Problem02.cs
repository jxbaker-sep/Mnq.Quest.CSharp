using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem02
{
  [Theory]
  [InlineData("|||||||", "O")]
  [InlineData("||||||", "E")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | ODD  _ R
ODD  | EVEN _ R
ODD  _ HALT O R
EVEN | ODD  _ R
EVEN _ HALT E R
";
}