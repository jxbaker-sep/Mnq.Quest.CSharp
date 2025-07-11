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
INIT | FOUND_ONE _ R

FOUND_ONE | START_COPY | R
FOUND_ONE * FINALIZE _ R

FINALIZE | FINALIZE | R
FINALIZE x FINALIZE | R
FINALIZE _ HALT _ R

START_COPY | START_COPY | R
START_COPY * FIND_COPYABLE * R

FIND_COPYABLE | COPY1 y R
FIND_COPYABLE x FULL_ERASING_REWIND x L

COPY1 | COPY1 | R
COPY1 x COPY1 x R
COPY1 _ REWIND x L

REWIND | REWIND | L
REWIND x REWIND x L
REWIND y FIND_COPYABLE y R

FULL_ERASING_REWIND y FULL_ERASING_REWIND | L
FULL_ERASING_REWIND * FULL_REWIND * L

FULL_REWIND | FULL_REWIND | L
FULL_REWIND * FULL_REWIND * L
FULL_REWIND _ INIT _ R
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
INIT _ REWIND = L

LOOKING | COPY1 _ R
LOOKING = HALT _ R

COPY1 | COPY1 | R
COPY1 = DUP22 = R
DUP22 | DUP22 | R
DUP22 _ DUP23 | R
DUP23 _ REWIND | L

REWIND | REWIND | L
REWIND = REWIND = L
REWIND _ LOOKING _ R
";
}