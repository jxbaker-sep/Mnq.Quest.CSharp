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
  [InlineData("|||||*||||||", "||||||||||||||||||||||||||||||")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | FOUND_ONE _ R
INIT * FINALIZE0 _ R

// We always double the count
FOUND_ONE | START_COPY _ R
FOUND_ONE * FINALIZE _ R

FINALIZE | FINALIZE | R
FINALIZE x FINALIZE | R
FINALIZE _ HALT _ R

FINALIZE0 | FINALIZE0 _ R
FINALIZE0 x FINALIZE0 | R
FINALIZE0 _ HALT _ R

START_COPY | START_COPY | R
START_COPY * FIND_COPYABLE * R

FIND_COPYABLE | FOUND1 y R
FIND_COPYABLE x FULL_ERASING_REWIND x L

FOUND1 | FOUND2 y R
FOUND1 x FOUND1 x R
FOUND1 _ WRITE1 x R // found 1, write 1 now then write 1 more

FOUND2 | FOUND2 | R
FOUND2 x FOUND2 x R
FOUND2 _ WRITE3 x R

WRITE1 _ REWIND x L
WRITE2 _ WRITE1 x R
WRITE3 _ WRITE2 x R

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