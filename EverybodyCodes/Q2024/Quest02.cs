using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;
using Mnq.Quest.CSharp.EverybodyCodes;

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
    List<string> allRunes = [.. input.Runes, .. input.Runes.Select(it => it.Reverse().Join(""))];
    foreach (var inscription in input.Inscription)
    {
      List<int> b = [];
      foreach (var rune in allRunes)
      {
        foreach (var index in FindAllIndexes(inscription, rune))
        {
          b.AddRange(Enumerable.Range(index, rune.Length));
        }
      }

      sum += b.Distinct().Count();
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

    Lexicon wordhoard = new();

    List<string> allRunes = [.. input.Runes, .. input.Runes.Select(it => it.Reverse().Join(""))];
    foreach (var rune in allRunes)
    {
      wordhoard.Add(rune);
    }

    HashSet<Point> runicScales = [];
    foreach (var (key, value) in grid.Items())
    {
      // Console.WriteLine(key);
      foreach (var vector in new[] { Vector.East, Vector.South })
      {
        HashSet<Point> path = [key];
        var current = key;
        var root = wordhoard.Follow(value);

        while (root is {})
        {
          if (root.IsWord)
          {
            runicScales.UnionWith(path);
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
          path.Add(current);
          root = root.Follow(grid[current]);
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


  public class Lexicon
  {
    private readonly Dictionary<char, Lexicon> Next = [];

    public bool IsWord { get; private set; } = false;

    public Lexicon? Follow(char c)
    {
      if (Next.TryGetValue(c, out var needle)) return needle;
      return null;
    }

    public void Add(string substring, int fromIndex = 0)
    {
      if (fromIndex >= substring.Length) {
        IsWord = true;
        return;
      }
      var next = Next.GetValueOrDefault(substring[fromIndex]) ?? new();
      Next[substring[fromIndex]] = next;
      next.Add(substring, fromIndex + 1);
    }
  }

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
