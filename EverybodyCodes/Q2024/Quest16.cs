using System.Drawing;
using System.Net.Http.Headers;
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
    var world = GetInput(inputFile);

    world.PullSizes.Zip(world.Wheels, (ps, wheel) => (ps, wheel))
      .Select(it => it.wheel[(100 * it.ps) % it.wheel.Count])
      .Join(" ")
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest16.2.Sample.txt", 280014668134)]
  [InlineData("Quest16.2.txt", 114778950421)]
  public void Part2(string inputFile, long expected)
  {
    var (pullSizes, wheels) = GetInput(inputFile);
    var current = pullSizes.Select(_ => 0).ToList();

    List<long> pullToScore = [];

    for(var i = 0 ; true; i++)
    {
      foreach(var (pullsize, index) in pullSizes.WithIndices()) current[index] = (current[index] + pullsize) % wheels[index].Count;
      var asString = current.Zip(wheels).Select(it => it.Second[it.First][0..1] + it.Second[it.First][2..3]).Join();
      var faces = asString.GroupToCounts();
      pullToScore.Add(faces.Values.Where(it => it >= 3).Sum(it => it - 2));
      if (current.All(it => it == 0)) break;
    }

    const long pulls = 202420242024;

    long cycles = pulls / pullToScore.Count;
    long last = cycles * pullToScore.Count;
    (pulls - last).Should().BeLessThan(pullToScore.Count);

    var result = pullToScore.Sum() * cycles + pullToScore.Take((int)(pulls - last)).Sum();
    result.Should().Be(expected);
  }

  private record World(List<int> PullSizes, List<List<string>> Wheels);

  private static World GetInput(string inputFile)
  {
    var lines = ECLoader.ReadLines(inputFile);
    var pullSize = lines[0].Split(',').Select(it => Convert.ToInt32(it)).ToList();
    List<List<string>> wheels = [.. pullSize.Select(_ => new List<string>{})];

    var padding = Enumerable.Repeat(' ', wheels.Count * 4).Join();
    foreach(var line in lines[2..])
    {
      var use = $"{line}{padding}"[..(wheels.Count * 4 - 1)];
      for(var x = 0; x < wheels.Count; x++)
      {
        var s = use[(x * 4)..(x * 4 + 3)];
        if (s == "   ") continue;
        wheels[x].Add(s);
      }
    }
    return new(pullSize, wheels);
  }

}
