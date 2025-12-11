
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;
using Microsoft.Z3;
using System.Runtime.InteropServices;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.LinearAlgebra.Complex;
using Microsoft.VisualBasic;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day11
{

  static Dictionary<string, List<string>> Parse(string file)
  {
    Dictionary<string, List<string>> result = [];
    foreach (var line in AdventOfCode2025Loader.ReadLines(file))
    {
      var a = line.Split(":");
      var b = a[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
      result[a[0]] = b.ToList();
    }
    return result;
  }

  [Theory]
  [InlineData("Day11.Sample.txt", 5)]
  [InlineData("Day11.txt", 494)]
  public void Part1(string inputFile, long expected)
  {
    var servers = Parse(inputFile);

    PathsFrom(servers, "you", "out", 0).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day11.Sample.2.txt", 2)]
  [InlineData("Day11.txt", 296006754704850)]
  public void Part2(string inputFile, long expected)
  {
    var servers = Parse(inputFile);

    PathsFrom(servers, "svr", "out", 3).Should().Be(expected);
  }

  public Dictionary<(string, string, long), long> Cache = [];
  const int fft = 1;
  const int dac = 2;
  public long PathsFrom(Dictionary<string, List<string>> servers, string start, string end, long required)
  {
    var key = (start, end, required);
    if (Cache.TryGetValue(key, out var cached)) return cached;

    if (start == "fft" && (required == 1 || required == 3)) required -= 1; 
    if (start == "dac" && (required == 2 || required == 3)) required -= 2; 

    long result = 0;
    foreach (var item in servers[start])
    {
      if (item == end && required == 0) result += 1;
      else if (item == end) {} // do nothing
      else result += PathsFrom(servers, item, end, required);
    }
    Cache[key] = result;
    return result;
  }
}


