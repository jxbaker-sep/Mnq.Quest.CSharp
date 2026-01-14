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

    world.PullSize.Zip(world.Wheels, (ps, wheel) => (ps, wheel))
      .Select(it => it.wheel[(100 * it.ps) % it.wheel.Count])
      .Join(" ")
      .Should().Be(expected);
  }

  private record World(List<int> PullSize, List<List<string>> Wheels);

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
