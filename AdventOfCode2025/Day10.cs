
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;

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

    machines.Sum(PressButtons).Should().Be(expected);
  }

  static long PressButtons(Machine machine)
  {
    var alloff = Enumerable.Repeat('.', machine.LightDiagram.Length).Join();

    Queue<(string, long)> open = new([(alloff, 0)]);
    HashSet<string> closed = [alloff];

    while (open.TryDequeue(out var current))
    {
      foreach (var b in machine.ButtonSchematics)
      {
        var next = PressButton(current.Item1, b);
        if (next == machine.LightDiagram) return current.Item2 + 1;
        if (!closed.Add(next)) continue;
        open.Enqueue((next, current.Item2 + 1));
      }
    }
    throw new ApplicationException();
  }

  static string PressButton(string currentLights, List<int> wireup)
  {
    var ca = currentLights.ToCharArray();
    foreach (var i in wireup) ca[i] = ca[i] == '.' ? '#' : '.';
    return ca.Join();
  }

}
