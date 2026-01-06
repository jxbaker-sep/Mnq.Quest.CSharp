using FluentAssertions;
using Utils;
using Mnq.Quest.CSharp.EverybodyCodes;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest09
{
  IReadOnlyList<IReadOnlyList<long>>  StampsCollection = [
    [10, 5, 3, 1],
    [30, 25, 24, 20, 16, 15, 10, 5, 3, 1],
  ];

  [Theory]
  // [InlineData("Quest09.1.Sample.txt", 0, 10)]
  // [InlineData("Quest09.1.txt", 0, 13603)]
  [InlineData("Quest09.2.Sample.txt", 1, 10)]
  // [InlineData("Quest09.2.txt", 1, 0)]
  public void Part1(string inputFile, int whichStamp, long expected)
  {
    var stamps = StampsCollection[whichStamp];
    var brightnesses = GetInput(inputFile);

    brightnesses.Sum(b =>
    {
      long result = 0;
      foreach(var stamp in stamps)
      {
        var n = b / stamp;
        b -= n * stamp;
        result += n;
      }
      return result;
    }).Should().Be(expected);
  }


  private static List<long> GetInput(string inputFile)
  {
    return ECLoader.ReadLines(inputFile).Select(it => Convert.ToInt64(it)).ToList();
  }

}
