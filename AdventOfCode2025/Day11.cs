
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

    PathsFrom(servers, "you", "out").Should().Be(expected);
  }

  public Dictionary<(string, string), long> Cache = [];
  public long PathsFrom(Dictionary<string, List<string>> servers, string start, string end)
  {
    var key = (start, end);
    if (Cache.TryGetValue(key, out var cached)) return cached;

    long result = 0;
    foreach (var item in servers[start])
    {
      if (item == end) result += 1;
      else result += PathsFrom(servers, item, end);
    }
    Cache[key] = result;
    return result;
  }
}


