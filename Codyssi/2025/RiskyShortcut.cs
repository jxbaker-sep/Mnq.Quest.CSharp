using Parser;
using FluentAssertions;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class RiskyShortcut
{
  [Theory]
  [InlineData("RiskyShortcut.Sample.txt", 52)]
  [InlineData("RiskyShortcut.txt", 4620)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    input.Sum(l => l.Count(c => char.IsAsciiLetter(c))).Should().Be(expected);
  }

  [Theory]
  [InlineData("RiskyShortcut.Sample.txt", 18)]
  [InlineData("RiskyShortcut.txt", 674)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    input.Select(Reduce).Sum(l => l.Length).Should().Be(expected);
  }

  [Theory]
  [InlineData("RiskyShortcut.Sample.txt", 26)]
  [InlineData("RiskyShortcut.txt", 1352)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    input.Select(Reduce2).Sum(l => l.Length).Should().Be(expected);
  }

  private string Reduce(string line)
  {
    Stack<char> s = [];
    foreach(var c in line)
    {
      if (s.TryPeek(out var pre) && (char.IsDigit(pre) ^ char.IsDigit(c)))
      {
        s.Pop();
      }
      else
      {
        s.Push(c);
      }
    }
    return s.Join();
  }

  private string Reduce2(string line)
  {
    Stack<char> s = [];
    foreach(var c in line)
    {
      if (c != '-' && s.TryPeek(out var pre) && pre != '-' && (char.IsDigit(pre) ^ char.IsDigit(c)))
      {
        s.Pop();
      }
      else
      {
        s.Push(c);
      }
    }
    return s.Join();
  }

  private static List<string> GetInput(string inputFile)
  {
    return CodyssiLoader.ReadLines(inputFile);
  }
}