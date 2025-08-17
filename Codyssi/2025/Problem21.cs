using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Microsoft.Z3;
using Mng.Quest.CSharp.Utils;
using System.Numerics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using FluentAssertions.Extensions;
using System.Security.Cryptography;

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

  [Theory]
  [InlineData("Problem21.Sample.1.txt", "1", "S1_0-S1_1-S1_2-S1_3-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "2", "S1_0-S1_1-S1_2-S1_3-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "3", "S1_0-S1_1-S1_2-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "4", "S1_0-S1_1-S1_2-S2_2-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "5", "S1_0-S1_1-S1_2-S2_2-S2_3-S1_3-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "6", "S1_0-S1_1-S1_2-S2_2-S2_3-S1_3-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "7", "S1_0-S1_1-S1_2-S2_2-S2_3-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "8", "S1_0-S1_1-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "9", "S1_0-S1_1-S2_3-S1_3-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "10", "S1_0-S1_1-S2_3-S1_3-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "11", "S1_0-S1_1-S2_3-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "12", "S1_0-S1_3-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "13", "S1_0-S1_3-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "14", "S1_0-S2_2-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "15", "S1_0-S2_2-S2_3-S1_3-S1_4-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "16", "S1_0-S2_2-S2_3-S1_3-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "17", "S1_0-S2_2-S2_3-S1_5-S1_6")]
  [InlineData("Problem21.Sample.1.txt", "100000000000000000000000000000", "S1_0-S2_2-S2_3-S1_5-S1_6")]
  [InlineData("Problem21.Sample.2.txt", "39", "S1_0-S1_1-S1_2-S2_3-S3_4-S3_5-S1_6")]
  [InlineData("Problem21.Sample.2.txt", "100000000000000000000000000000", "S1_0-S1_2-S2_3-S3_4-S3_5-S1_6")]
  [InlineData("Problem21.Sample.3.txt", "73287437832782344", "S1_0-S1_1-S1_2-S1_3-S1_4-S1_5-S1_6-S1_7-S1_8-S1_9-S1_10-S1_11-S1_12-S1_13-S1_14-S1_15-S1_16-S1_17-S1_18-S1_19-S1_20-S1_21-S1_22-S1_23-S1_24-S1_25-S1_26-S1_29-S5_29-S5_30-S5_35-S5_36-S5_37-S5_38-S5_39-S5_40-S5_45-S5_46-S5_47-S5_48-S5_51-S5_52-S5_53-S5_54-S5_55-S5_58-S5_59-S5_62-S5_63-S5_64-S5_65-S5_66-S5_67-S5_70-S5_71-S5_72-S1_76-S1_79-S1_80-S3_84-S3_85-S3_86-S3_87-S3_90-S1_92-S1_93-S1_94-S1_95-S1_98-S1_99")]
  [InlineData("Problem21.Sample.3.txt", "100000000000000000000000000000", "S1_0-S1_6-S2_11-S2_17-S2_23-S2_29-S9_34-S9_37-S5_42-S5_48-S5_54-S5_60-S5_66-S5_72-S5_73-S5_74-S1_79-S3_84-S8_88-S8_89-S8_90-S3_90-S3_91-S1_96-S1_99")]
  [InlineData("Problem21.txt", "100000000000000000000000000000", "S1_0-S1_1-S1_2-S1_3-S1_4-S1_5-S1_6-S1_7-S1_8-S1_9-S1_10-S1_11-S1_12-S55_13-S55_15-S55_16-S55_17-S55_18-S1_19-S1_22-S5_22-S5_23-S5_24-S5_25-S1_26-S1_27-S1_30-S1_32-S1_33-S1_36-S11_38-S11_39-S11_40-S11_41-S11_43-S11_45-S11_53-S11_54-S11_56-S11_57-S11_59-S11_61-S84_61-S84_62-S84_63-S84_64-S84_65-S84_66-S84_68-S30_70-S30_71-S11_71-S11_72-S11_73-S11_74-S67_80-S67_81-S67_83-S67_84-S67_86-S67_88-S67_89-S67_90-S67_91-S48_93-S48_94-S47_94-S47_95-S1_97")]
  public void Part3(string inputFile, string goal_rank_exp, string expected)
  {
    var goal_rank = BigInteger.Parse(goal_rank_exp);
    GetInput(inputFile);
    ComputeNumberOfPaths(Staircases[0], 0);
    var it = FindRank(Staircases[0], 0, 0, goal_rank);
    it.Should().Be(expected);
  }

  string FindRank(Staircase current, long step, BigInteger startingRank, BigInteger goalRank)
  {
    HashSet<(Staircase nextStaircase, long nextStep)> hashed_children = [];
    foreach (var move in Moves)
    {
      foreach (var (nextStaircase, nextStep) in Open(current, step, move))
      {
        hashed_children.Add((nextStaircase, nextStep));
      }
    }
    var children = hashed_children.OrderBy(it => it.nextStaircase.Id).ThenBy(it => it.nextStep).ToList();
    string needle = "";
    foreach (var (nextStaircase, nextStep) in children)
    {
      var np = ComputeNumberOfPaths(nextStaircase, nextStep);
      if (startingRank + np < goalRank)
      {
        startingRank += np;
        continue;
      }
      needle = FindRank(nextStaircase, nextStep, startingRank, goalRank);
      break;
    }
    if (needle == "")
    {
      if (children.Count == 0)
      {
        return $"{current.Name}_{step}";
      }
      var (nextStaircase, nextStep) = children[^1];
      var np = ComputeNumberOfPaths(nextStaircase, nextStep);
      needle = FindRank(nextStaircase, nextStep, startingRank, goalRank);
    }
    return $"{current.Name}_{step}-{needle}";
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

  public record Staircase(long Id, long FirstStep, long LastStep, string From, string To)
  {
    public string Name => $"S{Id}";
  }

  private void GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();
    var word = (P.Letter | P.Digit).Star().Join();
    var parser = P.Format("S{} : {} -> {} : FROM {} TO {}",
      P.Int, P.Long, P.Long, word, word)
      .Select(it => new Staircase(it.First, it.Second, it.Third, it.Fourth, it.Fifth));

    Staircases = parser.ParseMany(pps[0]);
    StaircaseMap = Staircases.ToDictionary(it => it.Name, it => it);
    Moves = P.Format("Possible Moves : {}", P.Long.Star(",")).Parse(pps[1][0]);
  }

  static string CalculateMD5(string data)
  {
    using (var md5 = MD5.Create())
    {
        var hash = md5.ComputeHash(data.Select(c => (byte)c).ToArray());
        return BitConverter.ToString(hash).ToLowerInvariant();
    }
  }
}
