using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem04
{
  [Theory]
  [InlineData("Problem04.Sample.txt", 7)]
  [InlineData("Problem04.txt", 49)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    GetGraph(input).Keys.Count().Should().Be(expected);

  }

  [Theory]
  [InlineData("Problem04.Sample.txt", 6)]
  [InlineData("Problem04.txt", 27)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    var g = GetGraph(input);
    Queue<string> open = [];
    open.Enqueue("STT");
    Dictionary<string, int> closed = [];
    closed["STT"] = 0;

    while (open.TryDequeue(out var c))
    {
      var d = closed[c];
      if (d == 3) continue;
      foreach(var next in g[c])
      {
        if (closed.TryAdd(next, d + 1))
        {
          open.Enqueue(next);
        }
      }
    }

    closed.Count(it => it.Value <= 3).Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem04.Sample.txt", 15)]
  [InlineData("Problem04.txt", 154)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    var g = GetGraph(input);
    Queue<string> open = [];
    open.Enqueue("STT");
    Dictionary<string, int> closed = [];
    closed["STT"] = 0;

    while (open.TryDequeue(out var c))
    {
      var d = closed[c];
      foreach(var next in g[c])
      {
        if (closed.TryAdd(next, d + 1))
        {
          open.Enqueue(next);
        }
      }
    }

    closed.Sum(it => it.Value).Should().Be(expected);
  }

  private static Dictionary<string, HashSet<string>> GetGraph(List<(string, string)> input)
  {
    Dictionary<string, HashSet<string>> d = [];
    foreach (var (a, b) in input)
    {
      d[a] = d.GetValueOrDefault(a) ?? [];
      d[b] = d.GetValueOrDefault(b) ?? [];
      d[a].Add(b);
      d[b].Add(a);
    }
    return d;
  }

  private static List<(string, string)> GetInput(string inputFile)
  {
    return P.Format("{}  <-> {}", P.Word, P.Word)
      .ParseMany(CodyssiLoader.ReadLines(inputFile));
  }
}
