
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day06
{
  [Theory]
  [InlineData("Day06.Sample.txt", 4277556)]
  [InlineData("Day06.txt", 5335495999141)]
  public void Part1(string inputFile, long expected)
  {
    var lines = AdventOfCode2025Loader.ReadLines(inputFile);
    var inputs = P.Long.Trim().Star().End().ParseMany(lines[..^1]);
    var ops = P.Any.Trim().Star().End().Parse(lines[^1]);

    var total = 0L;

    foreach (var (op, index) in ops.WithIndices())
    {
      var l = inputs.Select(l => l[index]).ToList();
      if (op == '*')
        total += l.Product();
      else
        total += l.Sum();
    }
    total.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day06.Sample.txt", 3263827)]
  [InlineData("Day06.txt", 10142723156431)]
  public void Part2(string inputFile, long expected)
  {
    var lines = AdventOfCode2025Loader.ReadLines(inputFile)
      .Select(it => it.ToList()).ToList().GridRotateLeft().Select(it => it.Join("")).ToList();

    var total = 0L;

    List<long> figures = [];
    foreach (var line in lines)
    {
      if (line.Trim() == "")
      {
        figures = [];
        continue;
      }
      else if (line.EndsWith('*'))
      {
        figures.Add(long.Parse(line[..^1].Join("").Trim()));
        total += figures.Product();
      }
      else if (line.EndsWith('+'))
      {
        figures.Add(long.Parse(line[..^1].Join("").Trim()));
        total += figures.Sum();
      }
      else
      {
        figures.Add(long.Parse(line.Trim()));
      }
    }
    total.Should().Be(expected);
  }

}