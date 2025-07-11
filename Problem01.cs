
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace Mng.Quest.CSharp;

public class Problem01
{
  [Theory]
  [InlineData("|||+||||", "|||||||")]
  [InlineData("|||||+||||||||", "|||||||||||||")]
  public void Part1(string input, string expected)
  {
    var result = new LogicMill(Program).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

  private string Program = @"
INIT | INIT | R
INIT + ADD | R
ADD | ADD | R
ADD _ BACKUP _ L
BACKUP | HALT _ R
";
}