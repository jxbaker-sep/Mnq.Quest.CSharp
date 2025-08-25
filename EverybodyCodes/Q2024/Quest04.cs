using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Data;
using Mnq.Quest.CSharp.EverybodyCodes;
using System.Net.Sockets;
using System.Xml;
using System.Linq.Expressions;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest04
{
  [Theory]
  [InlineData("Quest04.Sample.1.txt", 10)]
  [InlineData("Quest04.1.txt", 82)]
  [InlineData("Quest04.2.txt", 894899)]
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    var min = input.Min();
    input.Sum(it => it - min).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest04.Sample.3.txt", 8)]
  [InlineData("Quest04.3.txt", 122610071)]
  public void Part3(string inputFile, long expected)
  {
    var input = GetInput(inputFile);
    var total = long.MaxValue;
    foreach(var item in input)
    {
      var it2 = input.Sum(it => Math.Abs(it - item));
      total = Math.Min(total, it2);
    }
    total.Should().Be(expected);
  }

  private static List<long> GetInput(string inputFile)
  {
    return P.Long.ParseMany(ECLoader.ReadLines(inputFile));
  }
}
