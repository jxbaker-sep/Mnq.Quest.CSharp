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
      foreach (var nextVector in new[] { current.Vector, current.Vector.RotateLeft(), current.Vector.RotateRight() })
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

  [Theory]
  [InlineData("Quest20.2.Sample.1.txt", 24)]
  [InlineData("Quest20.2.Sample.2.txt", 78)]
  [InlineData("Quest20.2.Sample.3.txt", 206)]
  [InlineData("Quest20.2.txt", 538)] // 30s
  public void Part2(string inputFile, long expected)
  {
    const long startingAltitude = 10_000;

    var grid = GetInput(inputFile);
    var S = grid.Points((_, value) => value == 'S').First();
    var A = grid.Points((_, value) => value == 'A').First();
    var B = grid.Points((_, value) => value == 'B').First();
    var C = grid.Points((_, value) => value == 'C').First();
    List<Point> checkpoints = [A, B, C, S];
    Dictionary<Point, long> minimalRemainders = [];
    minimalRemainders[S] = 0;
    minimalRemainders[C] = C.ManhattanDistance(S);
    minimalRemainders[B] = B.ManhattanDistance(C) + minimalRemainders[C];
    minimalRemainders[A] = A.ManhattanDistance(B) + minimalRemainders[B];

    PriorityQueue<(Point Position, long Seconds, Vector Vector, Point Want, long Altitude)> open = new(current =>
    {
      var byCrowFlight = minimalRemainders[current.Want] + current.Position.ManhattanDistance(current.Want);
      var byAltitude = current.Altitude > startingAltitude ? (current.Altitude - startingAltitude) / 2 : startingAltitude - current.Altitude;
      return current.Seconds + Math.Max(byCrowFlight, byAltitude);
    });
    open.Enqueue((S, 0, Vector.South, A, startingAltitude));

    Dictionary<(Point Position, Vector Vector, Point Want, long Altitude), long> closed = [];

    while (open.TryDequeue(out var current))
    {
      foreach (var nextVector in new[] { current.Vector, current.Vector.RotateLeft(), current.Vector.RotateRight() })
      {
        var neighbor = current.Position + nextVector;
        var nextWant = current.Want;
        var currentAltitude = current.Altitude;
        var c = grid.Get(neighbor, Wall);
        if (c == Wall) continue;
        if (checkpoints.Contains(neighbor))
        {
          if (neighbor != current.Want) continue;
          if (neighbor == S)
          {
            if (currentAltitude - 1 == startingAltitude)
            {
              (current.Seconds + 1).Should().Be(expected);
              return;
            }
            continue;
          }
          else
          {
            nextWant = checkpoints[checkpoints.IndexOf(neighbor) + 1];
          }
        }

        var nextAltitude = currentAltitude + c switch
        {
          Cold => -2,
          Warm => +1,
          _ => -1,
        };
        if (nextAltitude <= 0) continue;
        if (closed.TryGetValue((neighbor, nextVector, nextWant, nextAltitude), out var foundSeconds) && foundSeconds <= current.Seconds + 1) continue;
        closed[(neighbor, nextVector, nextWant, nextAltitude)] = current.Seconds + 1;
        open.Enqueue((neighbor, current.Seconds + 1, nextVector, nextWant, nextAltitude));
      }

    }

    throw new ApplicationException();
  }

  [Theory]
  [InlineData("Quest20.3.Sample.txt", 100, 190)]
  [InlineData("Quest20.3.Sample.txt", 1000, 1990)]
  [InlineData("Quest20.3.Sample.txt", 10000, 19990)]
  [InlineData("Quest20.3.Sample.txt", 100000, 199990)]
  [InlineData("Quest20.3.Sample.txt", 384400, 768790)]
  [InlineData("Quest20.3.txt", 384400, 768792)]
  public void Part3(string inputFile, long currentAltitude, long expected)
  {
    var grid = GetInput(inputFile);
    var startingX = grid.Points((_, value) => value == 'S').First().X;
    var colsToHeights = Enumerable.Range(0, grid.Width)
      .Select(x => Enumerable.Range(0, grid.Height)
        .Sum(y => grid[y][x] switch
        {
          Cold => -2,
          Warm => +1,
          Wall => -1000,
          _ => -1
        })
      ).ToList();

    var delta = colsToHeights.Max();
    var all = colsToHeights.WithIndices().Where(it => it.Value == delta);
    var which = all.MinBy(it => Math.Abs(it.Index - startingX));

    currentAltitude -= Math.Abs(which.Index - startingX); // lose 1 altitude for adjusting position    
    // pretend we're starting 1 off screen from selected column
    long distance = -1;
    delta = Math.Abs(delta);
    currentAltitude += 1;
    while (currentAltitude > 0)
    {
      var n = currentAltitude / delta;
      if (n > 0) n -= 1;
      distance += n * grid.Height;
      currentAltitude -= n * delta;
      if (currentAltitude < delta * 2)
      {
        foreach (var y in Enumerable.Range(0, grid.Height))
        {
          if (currentAltitude == 0) break;
          currentAltitude += grid[y][which.Index] switch
          {
            Warm => +1,
            _ => -1 // other cases can't happen here
          };
          distance += 1;
        }
      }
    }

    distance.Should().Be(expected);
  }

  // [Theory]
  // // [InlineData("Quest20.3.Sample.txt", 1, 1)]
  // // [InlineData("Quest20.3.Sample.txt", 2, 2)]
  // // [InlineData("Quest20.3.Sample.txt", 3, 3)]
  // // [InlineData("Quest20.3.Sample.txt", 4, 4)]
  // // [InlineData("Quest20.3.Sample.txt", 5, 5)]
  // // [InlineData("Quest20.3.Sample.txt", 6, 6)]
  // // [InlineData("Quest20.3.Sample.txt", 7, 7)]
  // // [InlineData("Quest20.3.Sample.txt", 8, 9)]
  // // [InlineData("Quest20.3.Sample.txt", 9, 10)]
  // // [InlineData("Quest20.3.Sample.txt", 10, 11)]
  // [InlineData("Quest20.3.Sample.txt", 100, 190)]
  // // [InlineData("Quest20.3.Sample.txt", 1000, 1990)]
  // // [InlineData("Quest20.3.Sample.txt", 10000, 19990)]
  // // [InlineData("Quest20.3.Sample.txt", 100000, 199990)]
  // // [InlineData("Quest20.3.Sample.txt", 384400, 768790)]
  // public void Part3(string inputFile, long startingAltitude, long expected)
  // {
  //   var grid = GetInput(inputFile);
  //   var startingX = grid.Points((_, value) => value == 'S').First().X;
  //   grid[new Point(startingX, 0)] = Open;
  //   Dictionary<long, List<(Point Point, long DeltaAltitude)>> startingXToDestinations = [];

  //   for (var x = 0; x < grid.Width; x++)
  //   {
  //     startingXToDestinations[x] = CreateDestinations(grid, x, long.MaxValue / 2);
  //   }

  //   Queue<(long XPosition, long TotalDistance, long Altitude, long Iteration)> open = [];
  //   open.Enqueue((startingX, 0, startingAltitude, 0));
  //   // x position to distance
  //   Dictionary<(long x, long iteration), long> closed = [];

  //   long highWaterMark = 0;    

  //   while (open.TryDequeue(out var current))
  //   {
  //     var temp = startingXToDestinations[startingX];
  //     Console.WriteLine(current.Altitude);
  //     if (current.Altitude <= grid.Height * 2 )
  //     {
  //       temp = CreateDestinations(grid, current.XPosition, current.Altitude);
  //     }
  //     var possibles = temp
  //       .Where(it => it.DeltaAltitude + current.Altitude == 0 || (it.Point.Y == grid.Height - 1))
  //       .OrderBy(it => it.Point.Y)
  //       .ToList();

  //     foreach(var item in possibles)
  //     {
  //       var nextDistance = item.Point.Y + current.TotalDistance;
  //       var nextAltitude = current.Altitude + item.DeltaAltitude;
  //       if (item.DeltaAltitude + current.Altitude == 0)
  //       {
  //         highWaterMark = Math.Max(highWaterMark, nextDistance);
  //         continue;
  //       }
  //       if (item.Point.Y != grid.Height - 1) throw new ApplicationException("Unexpected non-end-row");
  //       nextDistance += 1; // move into next square
  //       nextAltitude -= 1;
  //       if (nextAltitude == 0) // we just moved down 1 into next square so we have a solution
  //       {
  //         highWaterMark = Math.Max(highWaterMark, nextDistance);
  //         continue;
  //       }
  //       if (closed.TryGetValue((item.Point.X, current.Iteration + 1), out var foundDistance) && foundDistance >= nextDistance) continue;
  //       closed[(item.Point.X, current.Iteration + 1)] = nextDistance;
  //       open.Enqueue((item.Point.X, nextDistance, nextAltitude, current.Iteration + 1));
  //     }
  //   }

  //   highWaterMark.Should().Be(expected);

  //   // throw new ApplicationException();
  // }

  // // Maps point to max delta altitide at that position
  // private List<(Point Point, long DeltaAltitude)> CreateDestinations(Grid<char> grid, long x, long startingAltitude)
  // {
  //   Point startingPosition = new(x, 0);
  //   Queue<(Point Position, Vector Vector)> open = new([(startingPosition, Vector.South)]);
  //   Dictionary<(Point Position, Vector Vector), long> closed = [];
  //   closed[(startingPosition, Vector.South)] = startingAltitude;

  //   while (open.TryDequeue(out var current))
  //   {
  //     var currentAltitude = closed[current];
  //     foreach (var nextVector in new[] { current.Vector, current.Vector.RotateLeft(), current.Vector.RotateRight() })
  //     {
  //       var neighbor = current.Position + nextVector;
  //       var c = grid.Get(neighbor, Wall);
  //       if (c == Wall) continue;
  //       var nextAltitude = currentAltitude + c switch
  //       {
  //         Cold => -2,
  //         Open => -1,
  //         Warm => +1,
  //         _ => throw new ApplicationException()
  //       };
  //       if (nextAltitude < 0) continue;
  //       if (closed.TryGetValue((neighbor, nextVector), out var foundAltitude) && foundAltitude >= nextAltitude) continue;
  //       closed[(neighbor, nextVector)] = nextAltitude;
  //       if (nextAltitude <= 0) continue;
  //       open.Enqueue((neighbor, nextVector));
  //     }
  //   }

  //   return closed.Select(it => (it.Key.Position, it.Value))
  //     .GroupBy(it => it.Position, it => it.Value)
  //     .Select(it => (it.Key, it.Max() - startingAltitude))
  //     .ToList();
  // }

  private static Grid<char> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Gridify();
  }
}
