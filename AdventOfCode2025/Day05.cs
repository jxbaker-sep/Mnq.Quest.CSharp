
using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;

namespace Mng.Quest.CSharp.AdventOfCode2025;

public class Day05
{


  [Theory]
  [InlineData("Day05.Sample.txt", 3)]
  [InlineData("Day05.txt", 885)]
  public void Part1(string inputFile, long expected)
  {
    var pps = AdventOfCode2025Loader.ReadAllText(inputFile).Paragraphs();

    var ranges = P.Format("{}-{}", P.Long, P.Long).End().ParseMany(pps[0]);
    var ingredients = P.Long.End().ParseMany(pps[1]);

    ingredients.Where(it => ranges.Any(r => r.First <= it && it <= r.Second))
      .Count().Should().Be((int)expected);
  }

  [Theory]
  [InlineData("Day05.Sample.txt", 14)]
  [InlineData("Day05.txt", 348115621205535)]
  public void Part2(string inputFile, long expected)
  {
    var pps = AdventOfCode2025Loader.ReadAllText(inputFile).Paragraphs();

    var ranges = P.Format("{}-{}", P.Long, P.Long).End().ParseMany(pps[0]);

    LinkedList<(long first, long second)> closed = [];

    foreach (var range in ranges)
    {
      var current = range;
      var link = closed.First;
      while (link != null)
      {
        var combined = TryCombineRanges(current, link.Value);
        if (combined is { } c)
        {
          var temp = link;
          link = link.Next;
          current = c;
          closed.Remove(temp);
          continue;
        }
        link = link.Next;
      }
      closed.AddLast(current);
    }


    closed.Sum(it => it.second - it.first + 1).Should().Be(expected);
  }

  static (long first, long second)? TryCombineRanges((long first, long second) one, (long first, long second) two)
  {
    if ((two.first <= one.first && one.first <= two.second) ||
        (one.first <= two.first && two.first <= one.second)
    )
    {
      return (Math.Min(one.first, two.first), Math.Max(one.second, two.second));
    }
    return null;
  }

}