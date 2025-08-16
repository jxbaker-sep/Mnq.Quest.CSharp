using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Microsoft.Z3;
using Mng.Quest.CSharp.Utils;
using System.Numerics;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem21
{
  [Theory]
  [InlineData("Problem21.Sample.1.txt", "6")]
  [InlineData("Problem21.Sample.2.txt", "13")]
  [InlineData("Problem21.Sample.3.txt", "231843173048269749794")]
  [InlineData("Problem21.txt", "90448546029334024540396217")]
  public void Part1(string inputFile, string rep_expected)
  {
    BigInteger expected = BigInteger.Parse(rep_expected);
    var items = GetInput(inputFile);
    ComputeNumberOfPaths(items.Staircases[0].FirstStep, items.Staircases[0].LastStep, items.Moves)
      .Should().Be(expected);
  }

  private readonly Dictionary<(long, long), BigInteger> Cache = [];
  BigInteger ComputeNumberOfPaths(long start, long stop, List<long> moves)
  {
    if (start == stop) return 1;
    var key = (start, stop);
    if (Cache.TryGetValue(key, out var needle)) return needle;
    BigInteger total = 0;
    foreach(var move in moves.Where(it => it <= stop - start))
    {
      total += ComputeNumberOfPaths(start + move, stop, moves);
    }
    Console.WriteLine($"{key}: {total}");
    Cache[key] = total;
    return total;
  }

  public record Staircase(string Name, long FirstStep, long LastStep, string From, string To);
  public record World(List<Staircase> Staircases, List<long> Moves);

  private static World GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();
    var word = (P.Letter | P.Digit).Star().Join();
    var parser = P.Format("{} : {} -> {} : FROM {} TO {}",
      word, P.Long, P.Long, word, word)
      .Select(it => new Staircase(it.First, it.Second, it.Third, it.Fourth, it.Fifth));

    return new(parser.ParseMany(pps[0]),
      P.Format("Possible Moves : {}", P.Long.Star(",")).Parse(pps[1][0]));
  }
}
