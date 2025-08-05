using Mng.Quest.CSharp.Utils;
using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class SirenDistruption
{
  [Theory]
  [InlineData("SirenDisruption.Sample.txt", 45)]
  [InlineData("SirenDisruption.txt", 31454)]
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    foreach(var (first, second) in input.Swaps)
    {
      (input.Frequencies[first], input.Frequencies[second]) = (input.Frequencies[second], input.Frequencies[first]);
    }

    input.Frequencies[input.TestIndex].Should().Be(expected);
  }

  [Theory]
  [InlineData("SirenDisruption.Sample.txt", 796)]
  [InlineData("SirenDisruption.txt", 56419)]
  public void Part2(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    foreach(var w in input.Swaps.Windows(2).Append([input.Swaps[^1], input.Swaps[0]]))
    {
      var first = w[0].Item1;
      var second = w[0].Item2;
      var third = w[1].Item1;
      (input.Frequencies[first], input.Frequencies[second], input.Frequencies[third]) = 
        (input.Frequencies[third], input.Frequencies[first], input.Frequencies[second]);
    }

    input.Frequencies[input.TestIndex].Should().Be(expected);
  }

  [Theory]
  [InlineData("SirenDisruption.Sample.txt", 827)]
  [InlineData("SirenDisruption.txt", 44634)]
  public void Part3(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    foreach(var (a, b) in input.Swaps)
    {
      var (first, second) = a < b ? (a, b) : (b, a);
      var fl = second - first;
      var sl = input.Frequencies.Count - second;
      var l = Math.Min(fl, sl);
      for (var i = 0; i < l; i++)
      {
        (input.Frequencies[first + i], input.Frequencies[second + i]) = 
          (input.Frequencies[second + i], input.Frequencies[first + i]);
      }
    }

    input.Frequencies[input.TestIndex].Should().Be(expected);
  }

  public record World(List<long> Frequencies, List<(int, int)> Swaps, int TestIndex);

  private static World GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();

    return new World(pps[0].Select(it => P.Long.Parse(it)).ToList(),
      pps[1].Select(it => P.Format("{}-{}", P.Int, P.Int).Select((it) => (it.First - 1, it.Second - 1)).Parse(it)).ToList(),
      P.Int.Parse(pps[2].Single()) - 1);
  }
}