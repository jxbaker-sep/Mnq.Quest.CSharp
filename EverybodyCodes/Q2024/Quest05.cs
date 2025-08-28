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
    for(var i = 0 ; i < 10; i++) Dance(columns, i);

    Shout(columns)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest05.Sample.2.txt", 50877075)]
  [InlineData("Quest05.2.txt", 17089733162436)]
  public void Part2(string inputFile, long expected)
  {
    var columns = GetInput(inputFile);

    Dictionary<long, long> counts = [];

    for (long turn = 0; ; turn++)
    {
      var current = Dance(columns, turn);
      var count = counts.GetValueOrDefault(current) + 1;
      if (count == 2024) {
        ((turn + 1) * current).Should().Be(expected);
        return;
      }
      counts[current] = count;
    }

    throw new ApplicationException();
  }

  [Theory]
  [InlineData("Quest05.Sample.3.txt", 6584)]
  [InlineData("Quest05.3.txt", 2693100410001002)]
  public void Part3(string inputFile, long expected)
  {
    var columns = GetInput(inputFile);

    Dictionary<string, long> cache = [];

    static string GetKey(List<LinkedList<long>> columns, int turn) => $"{turn}" + 
      columns.Select(c => c.Select(it => $"{it}").Join(",")).Join(";");

    cache[GetKey(columns, 0)] = 0;

    for (long turn = 0; ;turn++)
    {
      var current = Dance(columns, turn);
      var key = GetKey(columns, (int)(turn + 1) % 4);
      if (cache.TryGetValue(key, out var needle))
      {
        cache.Values.Max().Should().Be(expected);
        return;
      }
      cache[key] = current;
    }

    throw new ApplicationException();
  }

  private static long Shout(List<LinkedList<long>> columns)
  {
    return Convert.ToInt64(columns.Select(it => $"{it.First!.Value}").Join());
  }

  private static long Dance(List<LinkedList<long>> columns, long turn)
  {
    int selected = (int)(turn % 4);
    var next = columns[(selected + 1) % 4];
    var clapper = columns[selected].First();
    columns[selected].RemoveFirst();

    int index = (int)(clapper - 1) % (next.Count * 2);

    if (index < next.Count)
    {
      next.AddBefore(next.Nodes().Skip(index).First(), clapper);
    }
    else
    {
      next.AddAfter(next.Nodes().Skip(next.Count * 2 - index - 1).First(), clapper);
    }

    return Shout(columns);
  }

  private static List<LinkedList<long>> GetInput(string inputFile)
  {
    return P.Long.Trim().Star().ParseMany(ECLoader.ReadLines(inputFile)).Transpose()
      .Select(col => new LinkedList<long>(col)).ToList();
  }
}
