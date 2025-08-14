using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using System.Net;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem17
{
  [Theory]
  [InlineData("Problem17.Sample.txt", 36)]
  [InlineData("Problem17.txt", 3360)]
  public void Part1(string inputFile, long expected)
  {
    var graph = GetInput(inputFile);

    const string start = "STT";

    Queue<string> open = [];
    Dictionary<string, long> closed = [];
    closed[start] = 0;
    open.Enqueue(start);

    while (open.TryDequeue(out var current))
    {
      long distance = closed[current];
      foreach (var next in graph[current].Keys)
      {
        if (closed.ContainsKey(next)) continue;
        closed[next] = distance + 1;
        open.Enqueue(next);
      }
    }

    closed.Values.OrderByDescending(it => it).Take(3).Product().Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem17.Sample.txt", 44720)]
  [InlineData("Problem17.txt", 3226080)]
  public void Part2(string inputFile, long expected)
  {
    var graph = GetInput(inputFile);

    const string start = "STT";

    Queue<string> open = [];
    Dictionary<string, long> closed = [];
    closed[start] = 0;
    open.Enqueue(start);

    while (open.TryDequeue(out var current))
    {
      long distance = closed[current];
      foreach (var (next, step) in graph[current])
      {
        if (closed.TryGetValue(next, out var previous) && previous <= (distance + step)) continue;
        closed[next] = distance + step;
        open.Enqueue(next);
      }
    }

    closed.Values.OrderByDescending(it => it).Take(3).Product().Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem17.Sample.txt", 18)]
  [InlineData("Problem17.txt", 250)]
  public void Part3(string inputFile, long expected)
  {
    var graph = GetInput(inputFile);
    graph.Keys.Select(it => GetLongestCycle(it, graph)).Max().Should().Be(expected);
  }

  private static long GetLongestCycle(string start, Dictionary<string, Dictionary<string, long>> graph)
  {
    Queue<(List<string> Path, long Distance)> open = [];
    open.Enqueue(([start], 0));
    long needle = 0;

    while (open.TryDequeue(out var current))
    {
      var path = current.Path;
      var distance = current.Distance;
      foreach (var (next, step) in graph[path[^1]])
      {
        if (next == start) {
          needle = Math.Max(needle, distance + step);
          continue;
        }
        if (path.Contains(next)) continue;
        open.Enqueue(([.. path, next], distance + step));
      }
    }
    
    return needle;
  }


  private static Dictionary<string, Dictionary<string, long>> GetInput(string inputFile)
  {
    Dictionary<string, Dictionary<string, long>> result = [];
    foreach (var line in P.Format("{} -> {} | {}", P.Word, P.Word, P.Long)
      .ParseMany(CodyssiLoader.ReadLines(inputFile)))
    {
      result[line.First] = result.GetValueOrDefault(line.First) ?? [];
      result[line.Second] = result.GetValueOrDefault(line.Second) ?? [];
      result[line.First][line.Second] = line.Third;
    }
    return result;
  }
}
