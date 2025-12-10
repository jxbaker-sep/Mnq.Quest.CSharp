
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;
using Microsoft.Z3;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day10
{
  record Machine(string LightDiagram, List<List<int>> ButtonSchematics, List<long> JoltageRequirements);

  [Theory]
  [InlineData("Day10.Sample.txt", 7)]
  [InlineData("Day10.txt", 396)]
  public void Part1(string inputFile, long expected)
  {
    var lightDiagram = P.Format("[{}]", P.Choice(".", "#").Star().Join());
    var buttonSchematic = P.Format("({})", P.Int.Star(","));
    var joltageRequirements = P.Format("{{}}", P.Long.Star(","));

    var machine = P.Format("{}{}{}", lightDiagram, buttonSchematic.Star(), joltageRequirements)
    .Select(it => new Machine(it.First, it.Second, it.Third));

    var machines = machine.ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    machines.Sum(PressLightButtons).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day10.Sample.txt", 33)]
  [InlineData("Day10.txt", 0)]
  public void Part2(string inputFile, long expected)
  {
    var lightDiagram = P.Format("[{}]", P.Choice(".", "#").Star().Join());
    var buttonSchematic = P.Format("({})", P.Int.Star(","));
    var joltageRequirements = P.Format("{{}}", P.Long.Star(","));

    var machine = P.Format("{}{}{}", lightDiagram, buttonSchematic.Star(), joltageRequirements).End()
    .Select(it => new Machine(it.First, it.Second.OrderBy(it => it.Count).ToList(), it.Third));

    var machines = machine.ParseMany(AdventOfCode2025Loader.ReadLines(inputFile));

    // machines.Sum((machine) =>
    // {
    // Console.Error.WriteLine(machine.LightDiagram);
    // Cache.Clear();
    // return PressJoltageButtons(machine);
    // }).Should().Be(expected);

    machines.Sum(SolveForJoltage).Should().Be(expected);
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

  static long SolveForJoltage(Machine machine)
  {
    using Context ctx = new([]);
    // var ax = ctx.MkInt(machine.A.X);
    // var ay = ctx.MkInt(machine.A.Y);
    // var bx = ctx.MkInt(machine.B.X);
    // var by = ctx.MkInt(machine.B.Y);
    // var px = ctx.MkInt(machine.Prize.X);
    // var py = ctx.MkInt(machine.Prize.Y);

    // var apress = ctx.MkIntConst("apress");
    // var bpress = ctx.MkIntConst("bpress");

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
      .Where(it => it.Value.Contains(i)).Select(it => intConsts[it.Index]).ToArray();
      opt.Assert(ctx.MkEq(goal, ctx.MkAdd(exponents)));
    }

    if (opt.Check() != Status.SATISFIABLE) return 0;
    return intConsts.Sum(it => ((IntNum)opt.Model.ConstInterp(it)).Int64);
    // return ((IntNum)solver.Model.ConstInterp(apress)).Int64 * 3 + ((IntNum)solver.Model.ConstInterp(bpress)).Int64;
  }


  static long PressJoltageButtons(Machine machine)
  {
    var alloff = Enumerable.Repeat(0L, machine.JoltageRequirements.Count).ToList();

    var goal = machine.JoltageRequirements.Join(",");

    // PriorityQueue<(IReadOnlyList<long>, long)> open = new(it => it.Item2 + it.Item1.Zip(machine.JoltageRequirements)
    // .Max(a => a.Second - a.First));
    Stack<IReadOnlyList<long>> open = [];
    var max = long.MaxValue;
    open.Push(alloff);
    Dictionary<string, long> closed = [];
    closed[alloff.Join(",")] = 0;

    while (open.TryPop(out var current))
    {
      var key = current.Join(",");
      var presses = closed[key];
      if (presses >= max) continue;
      foreach (var b in machine.ButtonSchematics)
      {
        var next = PressJoltageButton(current, b);
        var nextKey = next.Join(",");
        if (closed.TryGetValue(nextKey, out var nextSteps) && nextSteps <= presses + 1) continue;
        closed[nextKey] = presses + 1;
        if (nextKey == goal) { max = Math.Min(max, presses + 1); continue; }
        if (next.Zip(machine.JoltageRequirements).Any(a => a.First > a.Second)) continue;
        open.Push(next);
      }
    }
    return max;
  }

  Dictionary<string, long?> Cache = [];
  long? PressJoltageButtons_old(Machine machine, IReadOnlyList<long> currentJoltage, long clue)
  {
    if (clue <= 0) return null;
    var key = currentJoltage.Join(",");

    if (Cache.TryGetValue(key, out var needle)) return needle;

    var goal = machine.JoltageRequirements.Join(",");

    long? recursiveBest = null;
    long nextClue = clue - 1;

    foreach (var bs in machine.ButtonSchematics)
    {
      var next = PressJoltageButton(currentJoltage, bs);
      if (next.Join(",") == goal)
      {
        recursiveBest = 0;
        break;
      }
      if (next.Zip(machine.JoltageRequirements).Any(a => a.First > a.Second || (a.Second - a.First) > nextClue)) continue;
      var recursive = PressJoltageButtons_old(machine, next, nextClue);
      if (recursive is { } rec && rec <= nextClue)
      {
        nextClue = rec;
        if (recursiveBest is { } res) recursiveBest = Math.Min(res, rec);
        else recursiveBest = recursive;
      }
    }

    Cache[key] = recursiveBest is { } ? recursiveBest + 1 : null;
    return Cache[key];
  }

  static List<long> PressJoltageButton(IReadOnlyList<long> currentJoltage, List<int> wireup)
  {
    List<long> copy = [.. currentJoltage];
    foreach (var i in wireup) copy[i] += 1;
    return copy;
  }

}


