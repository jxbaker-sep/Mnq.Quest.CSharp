using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Mng.Quest.CSharp.Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest07
{
  [Theory]
  [InlineData("Quest07.1.Sample.txt", "BDCA")]
  [InlineData("Quest07.1.txt", "ICKAJHFBG")]
  public void Part1(string inputFile, string expected)
  {
    var plans = GetInput(inputFile);

    plans.OrderByDescending(plan => Gather(plan, 10)).Select(it => it[0]).Join("").Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest07.2.Sample.txt", "Quest07.2.Track.Sample.txt", 1, "DCBA")]
  [InlineData("Quest07.2.Sample.txt", "Quest07.2.Track.Sample.txt", 10, "DCBA")]
  [InlineData("Quest07.2.txt", "Quest07.2.Track.txt", 10, "IACKFGDBJ")]
  public void Part2(string inputFile, string trackFile, int loops, string expected)
  {
    var plans = GetInput(inputFile);
    var track = GetTrack(trackFile);

    plans.OrderByDescending(plan => GatherOnTrack(plan.Skip(1).Join(), track, loops)).Select(it => it[0]).Join("").Should().Be(expected);
  }

  [Theory]
  [InlineData(4275)]
  public void Part3(long expected)
  {
    var rivalPlan = GetInput("Quest07.3.txt").Single();
    var track = GetTrack("Quest07.3.Track.txt");

    var rivalScore = GatherOnTrack(rivalPlan.Skip(1).Join(), track, 2024);

    CreateAllPlans(5, 3, 3).LongCount(plan =>
    {
      return GatherOnTrack(plan, track, 2024) > rivalScore;
    }).Should().Be(expected);
  }

  static IEnumerable<string> CreateAllPlans(int plus, int minus, int equals)
  {
    if (plus + minus + equals == 1)
    {
      if (plus > 0) yield return "+";
      if (minus > 0) yield return "-";
      if (equals > 0) yield return "=";
      yield break;
    }

    if (plus > 0)
    {
      foreach (var item in CreateAllPlans(plus - 1, minus, equals))
      {
        yield return $"+{item}";
      }
    }

    if (minus > 0)
    {
      foreach (var item in CreateAllPlans(plus, minus - 1, equals))
      {
        yield return $"-{item}";
      }
    }

    if (equals > 0)
    {
      foreach (var item in CreateAllPlans(plus, minus, equals - 1))
      {
        yield return $"={item}";
      }
    }
  }

  long GatherOnTrack(string plan, string track, int loops)
  {
    return GatherOnTrack2(plan, track, loops) + 10 * track.Length * loops;
  }

  Dictionary<string, (long Total, long Current)> Cache = [];

  long GatherOnTrack2(string plan, string track, int loops)
  {
    if (loops == 0) return 0;
    var key = plan;
    if (Cache.TryGetValue(key, out var found))
    {
      return found.Total + GatherOnTrack2(plan[(track.Length % plan.Length)..] + plan[..(track.Length % plan.Length)], track, loops-1) + (loops - 1) * found.Current * track.Length;
    }

    long total = 0;
    long current = 0;

    for (var step = 0; step < track.Length; step++)
    {
      var currentAction = plan[step % plan.Length];
      switch (track[step])
      {
        case 'S':
        case '=':
          break;
        case '+':
          currentAction = '+';
          break;
        case '-':
          currentAction = '-';
          break;
        default: throw new ApplicationException();
      }

      switch (currentAction)
      {
        case '+':
          current += 1;
          break;
        case '-':
          current -= 1;
          break;
      }
      total += current;
    }
    Cache[key] = (total, current);
    total += GatherOnTrack2(plan[(track.Length % plan.Length)..] + plan[..(track.Length % plan.Length)], track, loops-1);
    total += (loops - 1) * current * track.Length;
    return total;
  }

  static long Gather(List<string> plan, int segments)
  {
    var current = 10;
    var total = 0;
    for (int i = 0; i < segments; i++)
    {
      switch (plan[1 + (i % (plan.Count - 1))])
      {
        case "+":
          current += 1;
          break;
        case "-":
          current -= 1;
          break;
      }
      total += current;
    }
    return total;
  }

  private static List<List<string>> GetInput(string inputFile)
  {
    List<List<string>> result = [];
    foreach (var line in ECLoader.ReadLines(inputFile))
    {
      List<string> item = [];
      var temp = line.Split(":");
      item.Add(temp[0]);
      item.AddRange(temp[1].Split(','));
      result.Add(item);
    }
    return result;
  }

  private static string GetTrack(string inputFile)
  {
    var lines = ECLoader.ReadLines(inputFile);
    var result = "";
    var p = new Point(0, 0);
    var v = Vector.East;

    char at(Point p2) => p2.Y >= 0 && p2.Y < lines.Count && p2.X >= 0 && p2.X < lines[(int)p2.Y].Length ? lines[(int)p2.Y][(int)p2.X] : ' ';

    Dictionary<Vector, List<Vector>> Turns = [];
    Turns[Vector.North] = [Vector.East, Vector.West];
    Turns[Vector.South] = [Vector.East, Vector.West];
    Turns[Vector.East] = [Vector.North, Vector.South];
    Turns[Vector.West] = [Vector.North, Vector.South];

    while (result.Length == 0 || result[^1] != 'S')
    {
      var np = p + v;
      if (at(np) == ' ')
      {
        foreach (var turn in Turns[v])
        {
          if (at(p + turn) != ' ')
          {
            np = p + turn;
            v = turn;
            break;
          }
        }
      }
      result += at(np);
      p = np;
    }
    return result;
  }
}
