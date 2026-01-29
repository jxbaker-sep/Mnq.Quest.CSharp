using System.Diagnostics;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Z3;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public partial class Quest20
{
  const char Wall = '#';
  const char Open = '.';
  const char Cold = '-';
  const char Warm = '+';

  [Theory]
  [InlineData("Quest20.1.Sample.txt", 1045)]
  [InlineData("Quest20.1.txt", 1032)]
  public void Part1(string inputFile, long expected)
  {
    const long startingAltitude = 1_000;
    const long goalSeconds = 100;
    var grid = GetInput(inputFile);
    var startingPosition = grid.Points((_, value) => value == 'S').First();
    grid[startingPosition] = Wall;

    Queue<(Point Position, long Seconds, Vector Vector)> open = new([(startingPosition, 0, Vector.South)]);
    Dictionary<(Point Position, long Seconds, Vector Vector), long> closed = [];
    closed[(startingPosition, 0, Vector.South)] = startingAltitude;

    while (open.TryDequeue(out var current))
    {
      var currentAltitude = closed[current];
      foreach(var nextVector in new[]{current.Vector, current.Vector.RotateLeft(), current.Vector.RotateRight()})
      {
        var neighbor = current.Position + nextVector;
        var c = grid.Get(neighbor, Wall);
        if (c == Wall) continue;
        var nextAltitude = currentAltitude + c switch
        {
          Cold => -2,
          Open => -1,
          Warm => +1,
          _ => throw new ApplicationException()
        };
        if (nextAltitude <= 0) continue;
        if (closed.TryGetValue((neighbor, current.Seconds + 1, nextVector), out var found) && found >= nextAltitude) continue;
        closed[(neighbor, current.Seconds + 1, nextVector)] = nextAltitude;
        if (current.Seconds + 1 >= goalSeconds) continue;
        open.Enqueue((neighbor, current.Seconds + 1, nextVector));
      }
      
    }

    closed.Where(it => it.Key.Seconds == goalSeconds).Max(it => it.Value).Should().Be(expected);
  }


  private static Grid<char>  GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }
}
