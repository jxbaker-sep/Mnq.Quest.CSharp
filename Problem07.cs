using FluentAssertions;
using Utils;

namespace Mng.Quest.CSharp;

public class Problem07
{
  [Theory]
  [InlineData("cahach", "caha[ch]")]
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

    foreach (var token in lexicon) skip("INIT", token);
    foreach (var token in lexicon.Concat([sentinel, '[', ']'])) skip(rewind, token, "L");

    write("INIT", sentinel);
    var scan = NextState();
    rule(rewind, blank, scan, blank);
    halt(scan, sentinel);
    
    foreach (var token in lexicon)
    {
      var me = NextState();
      rule(scan, token, me, blank);
      foreach (var other in lexicon.Concat([sentinel, '[', ']']))
      {
        if (token == 'c' && other == 'h') {
          var ch = NextState();
          rule(me, other, ch, blank);
          foreach (var other2 in lexicon.Concat([sentinel, '[', ']'])) skip(ch, other2);
          writes(ch, "[ch]");
        }
        else skip(me, other);
      }
      if (token == 'w')
      {
        writes(me, $"[w]");
      }
      else
      {
        write(me, token);
      }
    }

    File.WriteAllLines("/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/Problem07.rules", rules);

    var result = new LogicMill(rules.Join("\n")).RunToHalt(input);
    result.Result.Should().Be(expected);
  }

}