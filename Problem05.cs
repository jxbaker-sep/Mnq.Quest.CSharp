using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem05
{
  [Theory]
  [InlineData("|:||", "||")]
  [InlineData("||:||,|||", "|||")]
  [InlineData("||:||,||||,|||", "||||")]
  [InlineData("||:|||,|||||,||||||||,||||", "|||||")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | FOUND_ONE _ R

FOUND_ONE | FOUND_TWO R
FOUND_ONE : FINALIZE _ R

FOUND_TWO | FOUND_TWO R
FOUND_TWO : REMOVE_FIRST R

FINALIZE | FINALIZE0 | R
FINALIZE _ FINALIZE _ R
FINALIZE0 | FINALIZE0 | R
FINALIZE0 , FINALIZE1 | R
FINALIZE0 _ HALT _ R
FINALIZE1 | FINALIZE1 _ R
FINALIZE1 , FINALIZE1 _ R
FINALIZE1 _ HALT _ R

REMOVE_FIRST | REMOVE_FIRST0 _ R
REMOVE_FIRST _ REMOVE_FIRST _ R
REMOVE_FIRST0 | REMOVE_FIRST0 _ R
REMOVE_FIRST0 , REWIND _ L

REWIND | REWIND | L
REWIND _ REWIND _ L
REWIND : REWIND0 : L
REWIND0 | REWIND0 | L
REWIND0 _ INIT _ R
";

}