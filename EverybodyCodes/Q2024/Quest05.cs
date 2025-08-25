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

public class Quest05
{
  [Theory]
  [InlineData("Quest05.Sample.1.txt", 2323)]
  [InlineData("Quest05.1.txt", 2325)]
  public void Part1(string inputFile, long expected)
  {
    var columns = GetInput(inputFile);
    for (var turn = 0; turn < 10; turn++)
    {
      var selected = turn % 4;
      var next = columns[(selected + 1) % 4];
      var clapper = columns[selected].First();
      columns[selected].RemoveFirst();

      if (clapper <= next.Count)
      {
        next.AddBefore(next.Nodes().Take((int)clapper).Last(), clapper);
      }
      else 
      {
        next.AddAfter(next.Nodes().Take(2 * next.Count - (int)clapper).Last(), clapper);
      }
    }

    columns.Select(it => it.First!.Value).Aggregate((a, b) => a * 10 + b)
      .Should().Be(expected);
  }

  private static List<LinkedList<long>> GetInput(string inputFile)
  {
    return P.Long.Trim().Star().ParseMany(ECLoader.ReadLines(inputFile)).Transpose()
      .Select(col => new LinkedList<long>(col)).ToList();
  }
}
