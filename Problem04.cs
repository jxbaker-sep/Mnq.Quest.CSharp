using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem04
{
  [Theory]
  [InlineData("||*|||", "||||||")]
  [InlineData("|*|", "|")]
  [InlineData("|*||", "||")]
  [InlineData("||*|", "||")]
  [InlineData("||*||", "||||")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | INIT | R
INIT * INIT * R
INIT _ FULL_REWIND y L

LOOKING2 | START_COPY _ R
LOOKING2 * MURDER _ R

MURDER | MURDER _ R
MURDER y HALT _ R

START_COPY | START_COPY | R
START_COPY * COPY * R

COPY | DUP21 z R
COPY y FULL_ERASING_REWIND y L

DUP21 | DUP21 | R
DUP21 y DUP21 y R
DUP21 _ REWIND | L

REWIND | REWIND | L
REWIND y REWIND y L
REWIND z COPY z R

FULL_ERASING_REWIND z FULL_ERASING_REWIND | L
FULL_ERASING_REWIND * FULL_REWIND * L

FULL_REWIND | FULL_REWIND | L
FULL_REWIND y FULL_REWIND y L
FULL_REWIND * FULL_REWIND * L
FULL_REWIND _ LOOKING2 _ R
";

  [Theory]
  [InlineData("|", "||")]
  [InlineData("||", "||||")]
  [InlineData("|||", "||||||")]
  public void Duplicate(string input, string expected)
  {
    var result = new LogicMill(DuplicateProgram).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

    private string DuplicateProgram = @"
INIT | INIT | R
INIT _ REWIND y L

LOOKING | DUP21 _ R
LOOKING y HALT _ R

DUP21 | DUP21 | R
DUP21 y DUP22 y R
DUP22 | DUP22 | R
DUP22 _ DUP23 | R
DUP23 _ REWIND | L

REWIND | REWIND | L
REWIND y REWIND y L
REWIND _ LOOKING _ R
";
}