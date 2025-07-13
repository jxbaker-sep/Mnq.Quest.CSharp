
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
using System.Diagnostics;

namespace Mng.Quest.CSharp;

public class Problem08: Program
{
  [Theory]
  [InlineData("hello-world", "dlrow-olleh")]
  public void Part2(string input, string expected)
  {
    string lexicon = Enumerable.Range('a', 'z' - 'a' + 1).Select(it => (char)it).Join() + "-äöõü";
    char sentinel = '=';
    char erased = '.';

    var ffw = CreateState();
    ffw.Skip(lexicon);
    ffw.On(sentinel, r => r.Then(Init));

    var finalize = CreateState();
    finalize.On(erased, r => r.Write(Blank).Left());
    finalize.On(sentinel, r => r.Write(Blank).Then(Halt));

    Init.Skip(erased);
    Init.On(Blank, r => r.Then(finalize).Left());
    Init.On(lexicon, r => {
      char token = r.Token;
      var copyBackNoSentinel = CreateState();
      copyBackNoSentinel.On(lexicon + erased, r => r.Left());
      var copyBack = CreateState();
      copyBackNoSentinel.On(Blank, r => r.Write(sentinel).Then(copyBack).Left());
      copyBackNoSentinel.On(sentinel, r => r.Left().Then(copyBack));
      copyBack.On(lexicon, r => r.Left());
      copyBack.On(Blank, r => r.Write(token).Right().Then(ffw));

      return r.Write(erased).Left().Then(copyBackNoSentinel);
    });

    Write("Problem08.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }
}