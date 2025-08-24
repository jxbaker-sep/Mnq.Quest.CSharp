using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;
using Mnq.Quest.CSharp.EverybodyCodes;
using System.Net.Sockets;
using System.Xml;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest02
{
  [Theory]
  [InlineData("Quest02.Sample.1.txt", 4)]
  [InlineData("Quest02.1.txt", 27)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    input.Runes.Select(rune => FindAllIndexes(input.Inscription[0], rune).Count()).Sum()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest02.Sample.2.txt", 42)]
  [InlineData("Quest02.2.txt", 5076)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    int sum = 0;
    foreach (var inscription in input.Inscription)
    {
      var b = inscription.Select(_ => false).ToList();
      foreach (var rune in input.Runes)
      {
        foreach (var index in FindAllIndexes(inscription, rune))
        {
          for (var i = index; i < index + rune.Length; i++)
          {
            b[i] = true;
          }
        }

        foreach (var index in FindAllIndexes(inscription.Reverse().Join(), rune))
        {
          for (var i = index; i < index + rune.Length; i++)
          {
            b[inscription.Length - i - 1] = true;
          }
        }
      }

      sum += b.Count(it => it);
    }

    sum.Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest02.Sample.3.txt", 10)]
  [InlineData("Quest02.3.txt", 11574)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    var grid = input.Inscription.Gridify();

    Dictionary<char, Thread>

    var d = input.Runes.GroupToDictionary(it => it[0]);
    foreach(var item in input.Runes)
    {
      var r = item.Reverse().Join();
      d[item[^1]] = d.GetValueOrDefault(item[^1]) ?? [];
      d[item[^1]].Add(r);
    }
    HashSet<Point> runicScales = [];
    foreach (var (key, value) in grid.Items())
    {
      if (!d.TryGetValue(value, out var currentRuneList)) continue;
      var max = currentRuneList.Max(it => it.Length);
      foreach (var vector in new[] { Vector.East, Vector.South })
      {
        var needle = "";
        HashSet<Point> found = [];
        var current = key;

        for (var i = 0; i < max; i++)
        {
          needle += grid[current];
          found.Add(current);
          if (currentRuneList.Contains(needle))
          {
            runicScales = [.. runicScales, .. found];
          }
          current += vector;
          if (!grid.Contains(current))
          {
            if (vector == Vector.East)
            {
              current = current with { X = 0 };
            }
            else break;
          }
        }

      }
    }


    runicScales.Should().HaveCount(expected);
  }

  static IEnumerable<int> FindAllIndexes(string haystack, string needle)
  {
    if (needle.Length == 0) throw new ApplicationException();
    var startIndex = 0;
    while (true)
    {
      startIndex = haystack.IndexOf(needle, startIndex);
      if (startIndex == -1) yield break;
      yield return startIndex;
      startIndex += 1;
    }
  }


  public record Thread(bool Word, Dictionary<char, Thread> Next);

  public record World(List<string> Runes, List<string> Inscription);

  private static World GetInput(string inputFile)
  {
    var lines = ECLoader.ReadLines(inputFile);
    return new World(P.Format("{}:{}", P.Word, P.Word.Star(","))
      .Select(it => it.Second)
      .Parse(lines[0]),
      lines[2..]);
  }
}
