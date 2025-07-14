using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem07 : Program
{
  [Theory]
  [InlineData("wõta-wastu-mu-soow-ja-chillitse-toomemäel", "[w]õta-[w]astu-mu-soo[w]-ja-[ch]illitse-toomemäel")]
  [InlineData("lühikese-aea-warul", "lühikese-aea-[w]arul")]
  public void Part1(string input, string expected)
  {
    const char sentinel = '=';
    string lexicon = Enumerable.Range('a', 'z' - 'a' + 1).Select(it => (char)it).Join() + "-äöõü";
    var rhsLexicon = lexicon.Concat(['[', ']']).Join();
    
    var rewind = CreateState("REWIND")
      .On(rhsLexicon.Append(sentinel).Join(), r => r.Left())
      .On(Blank, r => r.Then(Init));

    Init.On(sentinel, r => r.Write(Blank).Then(Halt));

    foreach (var token in lexicon)
    {
      var use = token == '-' ? '_' : token;
      var beforeSentinel = CreateState($"before_token_{use}");
      Init.On(token, r => r.Then(beforeSentinel).Write(Blank));

      beforeSentinel.Skip(lexicon);
      if (token == 'c') {
        var lookingForH = CreateState();

        Init.On('c', r => r.Write(Blank).Then(lookingForH), replace: true);
        lookingForH.On(lexicon, r => r.Then(beforeSentinel)); // go back to regular 'c'
        
        var foundHBeforeSentinel = CreateState();
        lookingForH.On('h', r => r.Write(Blank).Then(foundHBeforeSentinel), replace: true);

        foundHBeforeSentinel.Skip(lexicon);
        var foundHAfterSentinel = CreateState();
        foundHBeforeSentinel.On(sentinel, r => r.Then(foundHAfterSentinel));
        foundHAfterSentinel.Skip(rhsLexicon);
        foundHBeforeSentinel.On(Blank, r => r.Write($"{sentinel}[ch]", after => after.Then(rewind).Left()));
        foundHAfterSentinel.On(Blank, r => r.Write($"[ch]", after => after.Then(rewind).Left()));
      }
      var afterSentinel = CreateState($"after_token_{use}");
      beforeSentinel.On(sentinel, r => r.Then(afterSentinel));
      afterSentinel.Skip(rhsLexicon);

      var s = token == 'w' ? "[w]" : $"{token}";

      afterSentinel.On(Blank, r => r.Write(s, after => after.Left().Then(rewind)));
      beforeSentinel.On(Blank, r => r.Write($"{sentinel}{s}", after => after.Left().Then(rewind)));
    }

    Write("Problem07.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }
}