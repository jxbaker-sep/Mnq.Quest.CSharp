
using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Parser;
using P = Parser.ParserBuiltins;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day12
{

  [Theory]
  [InlineData("Day12.txt", 440)] 
  // [InlineData("Day12.Sample.1.txt", 528)] 
  public void Part1(string inputFile, int expected)
  {
    var pps = AdventOfCode2025Loader.ReadAllText(inputFile).Paragraphs();
    var instructions = P.Format("{}x{}: {}", P.Long, P.Long, P.Long.Trim().Star()).End().ParseMany(pps[^1]);

    List<int> sizes = [7, 7, 6, 7, 7, 5];
    // List<int> sizes = [5, 6, 7, 7, 7, 7];

    instructions.Count(it => it.First * it.Second >= it.Third.Zip(sizes).Sum(p => p.First * p.Second)).Should().Be(expected);
  }

}


