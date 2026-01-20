using System.Drawing;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using FluentAssertions;
using Microsoft.Z3;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest16
{
  [Theory]
  [InlineData("Quest16.1.Sample.txt", ">.- -.- ^,-")]
  [InlineData("Quest16.1.txt", "*.> >_< *,> *,-")]
  public void Part1(string inputFile, string expected)
  {
    GetInput(inputFile);

    StepsPerPull.Zip(Wheels, (ps, wheel) => (ps, wheel))
      .Select(it => it.wheel[(100 * it.ps) % it.wheel.Count])
      .Join(" ")
      .Should().Be(expected);
  }

  List<int> IncrementWheels(List<int> stepsPerPull, List<int> current)
  {
    foreach (var (pullsize, index) in stepsPerPull.WithIndices()) current[index] = MathUtils.MathMod(current[index] + pullsize, Wheels[index].Count);
    return current;
  }

  long ComputeScore(List<int> current)
  {
    var asString = current.Zip(Wheels).Select(it => it.Second[it.First][0..1] + it.Second[it.First][2..3]).Join("");
    var faces = asString.GroupToCounts();
    return faces.Values.Where(it => it >= 3).Sum(it => it - 2);
  }

  [Theory]
  [InlineData("Quest16.2.Sample.txt", 280014668134)]
  [InlineData("Quest16.2.txt", 114778950421)]
  public void Part2(string inputFile, long expected)
  {
    GetInput(inputFile);

    var current = StepsPerPull.Select(_ => 0).ToList();

    List<long> pullToScore = [];

    for (var i = 0; true; i++)
    {
      pullToScore.Add(ComputeScore(IncrementWheels(StepsPerPull, current)));
      if (current.All(it => it == 0)) break;
    }

    const long pulls = 202420242024;

    long cycles = pulls / pullToScore.Count;
    long last = cycles * pullToScore.Count;
    (pulls - last).Should().BeLessThan(pullToScore.Count);

    var result = pullToScore.Sum() * cycles + pullToScore.Take((int)(pulls - last)).Sum();
    result.Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest16.3.Sample.txt", 1, 4, 1)]
  [InlineData("Quest16.3.Sample.txt", 2, 6, 1)]
  [InlineData("Quest16.3.Sample.txt", 3, 9, 2)]
  [InlineData("Quest16.3.Sample.txt", 10, 26, 5)]
  [InlineData("Quest16.3.Sample.txt", 100, 246, 50)]
  [InlineData("Quest16.3.Sample.txt", 256, 627, 128)]
  [InlineData("Quest16.3.Sample.txt", 1000, 2446, 500)]
  // [InlineData("Quest16.3.Sample.txt", 2024, 4948, 1012)] // too much recursion
  [InlineData("Quest16.3.txt", 256, 630, 72)]
  public void Part3(string inputFile, long pulls, long expectedMax, long expectedMin)
  {
    GetInput(inputFile);

    var current = StepsPerPull.Select(_ => 0).ToList();

    IncrementAll = [.. StepsPerPull.Select(_ => 1)];
    DecrementAll = [.. StepsPerPull.Select(_ => -1)];
    DoPart3(current, pulls, true).Should().Be((expectedMax, expectedMin));
  }

  [Theory]
  [InlineData("Quest16.3.Sample.txt", 1, 4, 1)]
  [InlineData("Quest16.3.Sample.txt", 2, 6, 1)]
  [InlineData("Quest16.3.Sample.txt", 3, 9, 2)]
  [InlineData("Quest16.3.Sample.txt", 10, 26, 5)]
  [InlineData("Quest16.3.Sample.txt", 100, 246, 50)]
  [InlineData("Quest16.3.Sample.txt", 256, 627, 128)]
  [InlineData("Quest16.3.Sample.txt", 1000, 2446, 500)]
  [InlineData("Quest16.3.Sample.txt", 2024, 4948, 1012)]
  [InlineData("Quest16.3.txt", 256, 630, 72)]
  public void Part4(string inputFile, int totalPulls, long expectedMax, long expectedMin)
  {
    GetInput(inputFile);

    var zeroes = StepsPerPull.Select(_ => 0).ToList();

    IncrementAll = [.. StepsPerPull.Select(_ => 1)];
    DecrementAll = [.. StepsPerPull.Select(_ => -1)];

    Stack<(List<int> current, int pulls, bool allowLeftLever)> open = [];
    Dictionary<string, (long max, long min)> closed = [];
    open.Push((zeroes, totalPulls, true));

    string createKey(List<int> current, int pulls, bool allowLeftLever) => current.Join(',') + $"{pulls}" + $"{allowLeftLever}";
    while (open.TryPeek(out var temp))
    {
      var (current, pulls, allowLeftLever) = temp;
      var key = createKey(current, pulls, allowLeftLever);

      if (closed.ContainsKey(key)){ open.Pop(); continue;}

      if (pulls == 0)
      {
        closed[key] = (0, 0);
        open.Pop();
        continue;
      }

      if (allowLeftLever)
      {
        long max = long.MinValue;
        long min = long.MaxValue;
        var ok = true;
        
        foreach(var item in new[]{[], IncrementAll, DecrementAll})
        {
          List<int> c1 = item.Count > 0 ? IncrementWheels(item, [.. current]) : [..current];
          var k1 = createKey(c1, pulls, false);
          
          if (closed.TryGetValue(k1, out var d1))
          {
            max = Math.Max(d1.max, max);
            min = Math.Min(d1.min, min);
          }
          else
          {
            open.Push((c1, pulls, false));
            ok = false;
          }
        }

        if (ok) {closed[key] = (max, min); open.Pop();}
        continue;
      }

      current = IncrementWheels(StepsPerPull, [.. current]);

      var key2 = createKey([..current], pulls - 1, true);
      if (closed.TryGetValue(key2, out var found2))
      {
        var myScore = ComputeScore(current);
        closed[key] = (myScore + found2.max, myScore + found2.min);
        open.Pop();
      }
      else
      {
        open.Push(([..current], pulls - 1, true));
      }
    }

    var firstKey = zeroes.Join(',') + $"{totalPulls}" + $"{true}";
    closed[firstKey].Should().Be((expectedMax, expectedMin));
  }

  Dictionary<string, (long, long)> Cache = [];
  List<int> IncrementAll = [];
  List<int> DecrementAll = [];
  List<int> StepsPerPull = [];
  List<List<string>> Wheels = [];

  private (long max, long min) DoPart3(List<int> current, long pulls, bool allowLeftLever)
  {
    if (pulls == 0)
    {
      return (0, 0);
    }

    if (allowLeftLever)
    {
      var l = new[]{
        DoPart3([..current], pulls, false),
        DoPart3(IncrementWheels(IncrementAll, [..current]), pulls, false),
        DoPart3(IncrementWheels(DecrementAll, [..current]), pulls, false)
      };
      return (l.Max(it => it.max), l.Min(it => it.min));
    }

    var key = current.Join(',') + $"{pulls}";
    if (Cache.TryGetValue(key, out var found)) return found;
    current = IncrementWheels(StepsPerPull, [.. current]);

    var myScore = ComputeScore(current);
    var recursive = DoPart3(current, pulls - 1, true);
    var result = (myScore + recursive.max, myScore + recursive.min);
    Cache[key] = result;
    return result;
  }




  private void GetInput(string inputFile)
  {
    var lines = ECLoader.ReadLines(inputFile);
    StepsPerPull = lines[0].Split(',').Select(it => Convert.ToInt32(it)).ToList();
    Wheels = [.. StepsPerPull.Select(_ => new List<string> { })];

    var padding = Enumerable.Repeat(' ', Wheels.Count * 4).Join("");
    foreach (var line in lines[2..])
    {
      var use = $"{line}{padding}"[..(Wheels.Count * 4 - 1)];
      for (var x = 0; x < Wheels.Count; x++)
      {
        var s = use[(x * 4)..(x * 4 + 3)];
        if (s == "   ") continue;
        Wheels[x].Add(s);
      }
    }
  }

}
