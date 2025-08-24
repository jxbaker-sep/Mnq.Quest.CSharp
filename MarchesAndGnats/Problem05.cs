using FluentAssertions;
using Utils;

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

  [Theory]
  [InlineData("|:||", "||")]
  [InlineData("||:||,|||", "|||")]
  [InlineData("||:||,||||,|||", "||||")]
  [InlineData("||:|||,|||||,||||||||,||||", "|||||")]
  public void Part2(string input, string expected)
  {
    var rules = new List<string> { 
      "FINALIZE | FINALIZE _ R",
      "FINALIZE , FINALIZE _ R",
      "FINALIZE _ HALT _ R",
    };
    for (var i = 0; i < 256; i++)
    {
      // i is the number of prefix indices to erase
      var previous = i == 0 ? "INIT" : $"R{i - 1}";
      var me = $"R{i}";
      rules.Add($"{previous} | {me} _ R");
      rules.Add($"{me} : {me}a _ R");
      if (i == 0)
      {
        // this is the one to keep
        rules.Add($"{me}a | {me}a | R");
        // erase remainder
        rules.Add($"{me}a , FINALIZE _ R");
        // if no remainder, just halt
        rules.Add($"{me}a _ HALT _ R");
      }
      else{
        rules.Add($"{me}a | {me}a _ R");
        rules.Add($"{me}a , {previous}a _ R");
      }
    }
    var result = new LogicMill(rules.Join("\n")).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | FOUND_ONE _ R

FOUND_ONE | FOUND_TWO | R
FOUND_ONE : FINALIZE _ R

FOUND_TWO | FOUND_TWO | R
FOUND_TWO : REMOVE_FIRST : R

// Finalize: looking for first ground
FINALIZE | FINALIZE0 | R
FINALIZE _ FINALIZE _ R
// Finalize0: found first group, keeping it...
FINALIZE0 | FINALIZE0 | R
FINALIZE0 , FINALIZE1 _ R
FINALIZE0 _ HALT _ R
// Finalize1: scanned past first group, removing everything else...
FINALIZE1 | FINALIZE1 _ R
FINALIZE1 , FINALIZE1 _ R
FINALIZE1 _ HALT _ R

// REMOVE_FIRST: looking for first group to remove
REMOVE_FIRST | REMOVE_FIRST0 _ R
REMOVE_FIRST _ REMOVE_FIRST _ R
// REMOVE_FIRST0: found the group, remove it and trailing comma
REMOVE_FIRST0 | REMOVE_FIRST0 _ R
REMOVE_FIRST0 , REWIND _ L

// REWIND: scan back until we find :
REWIND | REWIND | L
REWIND _ REWIND _ L
REWIND : REWIND0 : L
// REWIND0: scan back until we find _ (start of tape)
REWIND0 | REWIND0 | L
REWIND0 _ INIT _ R
";

}