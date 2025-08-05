using FluentAssertions;
using P = Parser.ParserBuiltins;
using Parser;
using Utils;
namespace Mnq.Quest.CSharp.Codyssi;

public class Day03
{
  [Theory]
  [InlineData("Day03.Sample.txt", 43)]
  [InlineData("Day03.txt", 47310)]
  public void Part1(string inputFile, long expected)
  {
    var ranges = GetRanges(inputFile);

    ranges.Sum(it => it.Item1.Length + it.Item2.Length).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day03.Sample.txt", 35)]
  [InlineData("Day03.txt", 39070)]
  public void Part2(string inputFile, long expected)
  {
    var ranges = GetRanges(inputFile);

    ranges.SelectMany(it => it.Item1.Overlap(it.Item2)).Sum(it2 => it2.Length).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day03.Sample.txt", 9)]
  [InlineData("Day03.txt", 915)]
  public void Part3(string inputFile, long expected)
  {
    var ranges = GetRanges(inputFile);

    ranges.Windows(2).Select(w => Range.OverlapAll([w[0].Item1, w[0].Item2, w[1].Item1, w[1].Item2]).Sum(it=>it.Length)).Max().Should().Be(expected);
  }

  private record Range(long First, long Second)
  {
    public long Length => Second - First + 1;

    public List<Range> Overlap(Range other)
    {
      if (Second < other.First || First > other.Second)
      {
        return [this, other];
      }

      return [new Range(Math.Min(First, other.First), Math.Max(Second, other.Second))];
    }

    public static List<Range> OverlapAll(IEnumerable<Range> ranges)
    {
      var result = ranges.ToList();
      var skip = true;
      while (skip)
      {
        skip = false;
        for (var i = 0; !skip && i < result.Count; i++)
        {
          for (var j = i + 1; !skip && j < result.Count; j++)
          {
            var overlap = result[i].Overlap(result[j]);
            if (overlap.Count == 1) {
              result.RemoveAt(j);
              result.RemoveAt(i);
              result.Add(overlap[0]);
              skip = true;
            }
          }
        }
      }
      return result;
    }
  }

  private static IEnumerable<Tuple<Range, Range>> GetRanges(string inputFile)
  {
    return P.Format("{}-{} {}-{}", P.Long, P.Long, P.Long, P.Long)
          .Select(it => Tuple.Create(new Range(it.First, it.Second), new Range(it.Third, it.Fourth)))
          .ParseMany(CodyssiLoader.ReadLines(inputFile).ToList());
  }
}