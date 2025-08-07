using Mng.Quest.CSharp.Utils;
using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class WindyBargain
{
  [Theory]
  [InlineData("WindyBargain.Sample.txt", 2870)]
  [InlineData("WindyBargain.txt", 9456)]
  public void Part1(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    foreach(var t in input.Transactions)
    {
      input.Balances[t.Item1] -= t.Item3;
      input.Balances[t.Item2] += t.Item3;
    }

    input.Balances.OrderByDescending(it => it).Take(3).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("WindyBargain.Sample.txt", 2542)]
  [InlineData("WindyBargain.txt", 5402)]
  public void Part2(string inputFile, int expected)
  {
    var input = GetInput(inputFile);

    foreach(var t in input.Transactions)
    {
      var amt = Math.Min(t.Item3, input.Balances[t.Item1]);
      input.Balances[t.Item1] -= amt;
      input.Balances[t.Item2] += amt;
    }

    input.Balances.OrderByDescending(it => it).Take(3).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("WindyBargain.Sample.txt", 2511)]
  [InlineData("WindyBargain.txt", 0)]
  public void Part3(string inputFile, int expected)
  {
    var input = GetInput(inputFile);
    List<List<Debt>> debts = input.Balances.Select(it => new List<Debt>()).ToList();

    foreach(var t in input.Transactions)
    {
      var debt = debts[t.Item2];
      var amt = t.Item3;
      for (var i = 0; i < debt.Count && amt > 0; i++)
      {
        var d = debt[i];
        if (d.Amount == 0) continue;
        var paidOff = Math.Min(d.Amount, amt);
        amt -= paidOff;
        d.Amount -= paidOff;
        input.Balances[d.To] += paidOff;
      }
      amt = Math.Min(t.Item3, amt);
      input.Balances[t.Item1] -= amt;
      input.Balances[t.Item2] += amt;
      if (amt < t.Item3)
      {
        debts[t.Item1].Add(new(t.Item2, t.Item3 - amt));
      }
    }

    input.Balances.OrderByDescending(it => it).Take(3).Sum().Should().Be(expected);
  }

  public struct Debt(int To, int Amount)
  {
    public int To { get; set; } = To;
    public int Amount { get; set; } = Amount;
  }

  public record World(List<int> Balances, List<(int, int, int)> Transactions);

  private static World GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();

    int WordToIndex(string w) => w[0] - 'A';

    var id = P.String("X-ray") | P.Word;
    return new World(pps[0].Select(it => P.Format("{} HAS {}", id, P.Int).Select(it => it.Second).Parse(it)).ToList(),
      pps[1].Select(it => P.Format("FROM {} TO {} AMT {}", id, id, P.Int).Select((it) => (WordToIndex(it.First), WordToIndex(it.Second), it.Third)).Parse(it)).ToList());
  }
}