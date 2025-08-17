using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Microsoft.Z3;
using Mng.Quest.CSharp.Utils;
using System.Numerics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

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
    GetInput(inputFile);
    Staircases = [Staircases[0]];
    ComputeNumberOfPaths(Staircases[0], 0)
    .Should().Be(expected);

    // foreach (var path in Paths.Values.SelectMany(it => it).OrderBy(it => it)) Console.WriteLine(path);

  }

  private IReadOnlyList<long> Moves = [];
  private IReadOnlyList<Staircase> Staircases = [];
  private IReadOnlyDictionary<string, Staircase> StaircaseMap = new Dictionary<string, Staircase>();

  [Theory]
  [InlineData("Problem21.Sample.1.txt", "17")]
  [InlineData("Problem21.Sample.2.txt", "102")]
  [InlineData("Problem21.Sample.3.txt", "113524314072255566781694")]
  [InlineData("Problem21.txt", "411868671461623512011293423204353")]
  public void Part2(string inputFile, string rep_expected)
  {
    BigInteger expected = BigInteger.Parse(rep_expected);
    GetInput(inputFile);
    var result = ComputeNumberOfPaths(Staircases[0], 0);

    // foreach (var path in Paths.Values.SelectMany(it => it).OrderBy(it => it)) Console.WriteLine(path);
    result.Should().Be(expected);
  }

  private readonly Dictionary<string, BigInteger> PathCache = [];
  // private readonly Dictionary<string, List<string>> Paths = [];
  BigInteger ComputeNumberOfPaths(Staircase current, long step)
  {
    var key = $"{current.Name}_{step}";
    if (PathCache.TryGetValue(key, out var needle)) return needle;
    // Paths[key] = Paths.GetValueOrDefault(key) ?? [];
    if (current.To == "END" && step == current.LastStep)
    {
      PathCache[key] = 1;
      // Paths[key].Add(key);
      return 1;
    }

    BigInteger total = 0;
    HashSet<string> closed = [];
    foreach (var move in Moves)
    {
      foreach (var (nextStaircase, nextStep) in Open(current, step, move))
      {
        if (!closed.Add($"{nextStaircase}_{nextStep}")) continue;
        var temp = ComputeNumberOfPaths(nextStaircase, nextStep);
        total += ComputeNumberOfPaths(nextStaircase, nextStep);
        // Paths[key].AddRange(Paths[$"{nextStaircase.Name}_{nextStep}"].Select(it => $"{key}->{it}"));
      }
    }
    // Console.WriteLine($"{current.Name} {step} = {total}");

    PathCache[key] = total;
    return total;
  }

  private Dictionary<(string, long, long), IReadOnlySet<(Staircase, long)>> OpenCache = [];
  private IReadOnlySet<(Staircase, long)> Open(Staircase current, long step, long move)
  {
    HashSet<(Staircase, long)> result = [];
    if (move == 0)
    {
      result.Add((current, step));
      return result;
    }
    var key = (current.Name, step, move);
    if (OpenCache.TryGetValue(key, out var needle)) return needle;
    if (step < current.LastStep)
    {
      result = [.. result, .. Open(current, step + 1, move - 1)];
    }
    if (step == current.LastStep && current.To != "END")
    {
      result = [.. result, .. Open(StaircaseMap[current.To], step, move - 1)];
    }
    var neighbors = Staircases.Where(sc => sc.From == current.Name && sc.FirstStep == step).ToList();
    // neighbors.Should().HaveCountLessThanOrEqualTo(1); // sanity
    foreach (var next in neighbors)
    {
      result = [.. result, .. Open(next, step, move - 1)];
    }
    OpenCache[key] = result;
    return result;
  }

  public record Staircase(string Name, long FirstStep, long LastStep, string From, string To);

  private void GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();
    var word = (P.Letter | P.Digit).Star().Join();
    var parser = P.Format("{} : {} -> {} : FROM {} TO {}",
      word, P.Long, P.Long, word, word)
      .Select(it => new Staircase(it.First, it.Second, it.Third, it.Fourth, it.Fifth));

    Staircases = parser.ParseMany(pps[0]);
    StaircaseMap = Staircases.ToDictionary(it => it.Name, it => it);
    Moves = P.Format("Possible Moves : {}", P.Long.Star(",")).Parse(pps[1][0]);
  }
}
