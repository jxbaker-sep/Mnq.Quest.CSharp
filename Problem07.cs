using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem07
{
  [Theory]
  [InlineData("wõta-wastu-mu-soow-ja-chillitse-toomemäel", "[w]õta-[w]astu-mu-soo[w]-ja-[ch]illitse-toomemäel")]
  public void Part1(string input, string expected)
  {
    List<string> rules = [];

    const char sentinel = '=';
    const char blank = '_';
    const string left = "L";
    string lexicon = Enumerable.Range('a', 'z' - 'a' + 1).Select(it => (char)it).Join() + "-äöõü";

    var r = 0;

    string NextState() => $"r{r++}";

    var rewind = "REWIND";

    HashSet<(string, char)> closed = [];

    void rule(string state, char token, string next, char repl, string dir = "R")
    {
      if (!closed.Add((state, token))) throw new ApplicationException($"Already inserted rule ({(state, token)})");
      rules.Add($"{state} {token} {next} {repl} {dir}");
    }

    void write(string state, char token) {
      rule(state, blank, rewind, token, left);
    }
    void writes(string state, string tokens) {
      foreach(var (token, index) in tokens.WithIndices()) {
        var next = index == tokens.Length - 1 ? rewind : NextState();
        var dir = next == rewind ? "L" : "R";
        rule(state, blank, next, token, dir);
        state = next;
      }
    }
    void skip(string state, char token, string dir = "R") {
      rule(state, token, state, token, dir);
    }
    void halt(string state, char token, char repl=blank) {
      rule(state, token, "HALT", repl);
    }

    foreach (var token in lexicon.Concat([sentinel, '[', ']'])) skip(rewind, token, "L");

    var scan = "INIT";
    rule(rewind, blank, scan, blank);
    halt(scan, sentinel);
    
    foreach (var token in lexicon)
    {
      var meNoSentinel = NextState();
      rule(scan, token, meNoSentinel, blank);
      foreach (var other in lexicon.Concat(['[', ']']))
      {
        if (token == 'c' && other == 'h') {
          var chNoSentinel = NextState();
          rule(meNoSentinel, other, chNoSentinel, blank);
          foreach (var other2 in lexicon.Concat(['[', ']'])) skip(chNoSentinel, other2);
          var ch = NextState();
          foreach (var other2 in lexicon.Concat(['[', ']'])) skip(ch, other2);
          rule(chNoSentinel, sentinel, ch, sentinel);
          writes(ch, "[ch]");
          writes(chNoSentinel, $"{sentinel}[ch]");
        }
        else skip(meNoSentinel, other);
      }
      var me = NextState();
      rule(meNoSentinel, sentinel, me, sentinel);
      foreach (var other2 in lexicon.Concat(['[', ']'])) skip(me, other2);

      if (token == 'w')
      {
        writes(me, $"[w]");
        writes(meNoSentinel, $"{sentinel}[w]");
      }
      else
      {
        write(me, token);
        writes(meNoSentinel, $"{sentinel}{token}");
      }
    }

    File.WriteAllLines("/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/Problem07.rules", rules);

    var result = new LogicMill(rules.Join("\n")).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

}