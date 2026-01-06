
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Utils;
using Microsoft.Z3;
using System.Runtime.InteropServices;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day10
{
  record Machine(string LightDiagram, List<List<int>> ButtonSchematics, List<int> JoltageRequirements);

  static List<Machine> Parse(string file)
  {
    var lightDiagram = P.Format("[{}]", P.Choice(".", "#").Star().Join());
    var buttonSchematic = P.Format("({})", P.Int.Star(","));
    var joltageRequirements = P.Format("{{}}", P.Int.Star(","));

    var machine = P.Format("{}{}{}", lightDiagram, buttonSchematic.Star(), joltageRequirements)
      .Select(it => new Machine(it.First, it.Second, it.Third));

    return machine.ParseMany(AdventOfCode2025Loader.ReadLines(file));
  }

  [Theory]
  [InlineData("Day10.Sample.txt", 7)]
  [InlineData("Day10.txt", 396)]
  public void Part1(string inputFile, long expected)
  {
    var machines = Parse(inputFile);

    machines.Sum(PressLightButtons).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day10.Sample.txt", 33)]
  [InlineData("Day10.txt", 15688)]
  public void Part2ViaZ3(string inputFile, long expected)
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      // Z3 doesn't work on mac for me...
      return;
    }
    var machines = Parse(inputFile);

    machines.Sum(Z3SolveForJoltage).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day10.Sample.txt", 33)]
  [InlineData("Day10.txt", 15688)]
  public void Part2ViaSolving(string inputFile, long expected)
  {
    var machines = Parse(inputFile);

    machines.WithIndices().Sum((a) =>
    {
      var machine = a.Value;
      var index = a.Index;
      Cache.Clear();
      Cache2.Clear();
      Console.WriteLine($"{machine.LightDiagram} {index} / {machines.Count}");
      return (long)(RecursivelySolveJoltages(machine.ButtonSchematics, 0, machine.JoltageRequirements, int.MaxValue) ?? throw new ApplicationException());
    }).Should().Be(expected);
  }

    readonly Dictionary<string, int?> Cache = [];
  public int? RecursivelySolveJoltages(IReadOnlyList<List<int>> buttons, int joltageIndex, IReadOnlyList<int> joltagesRemaining, int hint)
  {
    var key = joltagesRemaining.Join(",");

    if (Cache.TryGetValue(key, out var needle)) return needle;

    var nextbuttons = buttons.Where(b => !b.Contains(joltageIndex)).ToList();
    if (joltagesRemaining[joltageIndex] == 0)
    {
      if (nextbuttons.Count == 0) { Cache[key] = 0; return 0; }
      var x = RecursivelySolveJoltages(nextbuttons, joltageIndex + 1, joltagesRemaining, hint);
      Cache[key] = x;
      return x;
    }
    var inuse = buttons.Where(b => b.Contains(joltageIndex)).ToList();
    if (inuse.Count == 0)
    {
      Cache[key] = null;
      return null;
    }

    int? result = null;
    foreach (var presses in RecursivelyCreatePresses(inuse, 0, joltageIndex, joltagesRemaining))
    {
      var currentResult = presses.Sum();
      if (result is { } r && currentResult >= r) continue;
      if (currentResult >= hint) continue;
      List<int> nextJoltagesRemaining = [.. joltagesRemaining];
      foreach (var (timesPressed, buttonIndex) in presses.ToList().WithIndices())
      {
        foreach (var wire in inuse[buttonIndex])
        {
          nextJoltagesRemaining[wire] -= timesPressed;
        }
      }
      if (nextJoltagesRemaining.Any(joltage => joltage < 0)) continue;
      if (nextbuttons.Count > 0)
      {
        var recursive = RecursivelySolveJoltages(nextbuttons, joltageIndex + 1, nextJoltagesRemaining, result == null ? hint - currentResult : (int)result - currentResult);
        if (recursive is int r2) currentResult += r2;
        else continue;
      }
      result = Math.Min(result ?? int.MaxValue, currentResult);
    }

    Cache[key] = result;
    return result;
  }

  Dictionary<string, List<Stack<int>>> Cache2 = [];
  public List<Stack<int>> RecursivelyCreatePresses(IReadOnlyList<List<int>> inuse, int buttonIndex, int remainderIndex, IReadOnlyList<int> remainders)
  {
    var key = $"{buttonIndex}-${remainderIndex}-${remainders.Join(",")}";

    if (Cache2.TryGetValue(key, out var found)) return [.. found.Select(it => new Stack<int>(it))];

    List<Stack<int>> result = [];
    var max = inuse[buttonIndex].Min(wire => remainders[wire]);
    if (buttonIndex == inuse.Count - 1)
    {
      if (max >= remainders[remainderIndex])
      {
        result.Add(new([remainders[remainderIndex]]));
      }
    }
    else
    {
      for (var i = 0; i <= max; i++)
      {
        var newRemainders = remainders.Select((it, index) => inuse[buttonIndex].Contains(index) ? it - i : it).ToList();
        foreach (var next in RecursivelyCreatePresses(inuse, buttonIndex + 1, remainderIndex, newRemainders))
        {
          next.Push(i);
          result.Add(next);
        }
      }
    }

    Cache2[key] = result;
    return result;
  }

  static long PressLightButtons(Machine machine)
  {
    var alloff = Enumerable.Repeat('.', machine.LightDiagram.Length).Join();

    Queue<(string, long)> open = new([(alloff, 0)]);
    HashSet<string> closed = [alloff];

    while (open.TryDequeue(out var current))
    {
      foreach (var b in machine.ButtonSchematics)
      {
        var next = PressLightButton(current.Item1, b);
        if (next == machine.LightDiagram) return current.Item2 + 1;
        if (!closed.Add(next)) continue;
        open.Enqueue((next, current.Item2 + 1));
      }
    }
    throw new ApplicationException();
  }

  static string PressLightButton(string currentLights, List<int> wireup)
  {
    var ca = currentLights.ToCharArray();
    foreach (var i in wireup) ca[i] = ca[i] == '.' ? '#' : '.';
    return ca.Join();
  }

  static long Z3SolveForJoltage(Machine machine)
  {
    using Context ctx = new([]);

    var intConsts = machine.ButtonSchematics.Select((it, index) => ctx.MkIntConst($"press{index}")).ToList();

    // Add constraints to the consts
    using var opt = ctx.MkOptimize();
    foreach (var (bs, index) in machine.ButtonSchematics.WithIndices())
    {
      var intConst = intConsts[index];
      opt.Add(ctx.MkGe(intConst, ctx.MkInt(0)));
      opt.Add(ctx.MkLe(intConst, ctx.MkInt(bs.Min(it => machine.JoltageRequirements[it]))));
    }

    opt.MkMinimize(ctx.MkAdd(intConsts));

    // var solver = ctx.MkSimpleSolver();
    foreach (var i in machine.JoltageRequirements.WithIndices().Select(it => it.Index))
    {
      var goal = ctx.MkInt(machine.JoltageRequirements[i]);
      ArithExpr[] exponents = machine.ButtonSchematics.WithIndices()
        .Where(it => it.Value.Contains(i))
        .Select(it => intConsts[it.Index])
        .ToArray();
      opt.Assert(ctx.MkEq(goal, ctx.MkAdd(exponents)));
    }

    if (opt.Check() != Status.SATISFIABLE) return 0;
    return intConsts.Sum(it => ((IntNum)opt.Model.ConstInterp(it)).Int64);
  }

}


