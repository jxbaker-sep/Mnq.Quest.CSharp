using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using System.Net;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem16
{
  [Theory]
  [InlineData("Problem16.Sample.txt", 18938)]
  [InlineData("Problem16.txt", 20176623862)] 
  public void Part1(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    foreach (var op in input.Instructions)
    {
      op.Operate(input.Grid);
    }

    input.Grid.Concat(input.Grid.Transpose())
        .Max(row => row.Select(it => it.Value).Sum())
        .Should()
        .Be(expected);
  }

  [Theory]
  [InlineData("Problem16.Sample.txt", 11496)]
  [InlineData("Problem16.txt", 35722143)] 
  public void Part2(string inputFile, long expected)
  {
    var input = GetInput(inputFile);

    var instructions = new Queue<IInstruction>(input.Instructions);

    IInstruction taken = new ShiftBy(0, new ColSelector(0));

    foreach (var op in input.Flow)
    {
      if (op == "TAKE") {
        taken = instructions.Dequeue();
      }
      else if (op == "CYCLE") {
        instructions.Enqueue(taken);
      }
      else if (op == "ACT") {
        taken.Operate(input.Grid);
      }
    }

    input.Grid.Concat(input.Grid.Transpose())
        .Max(row => row.Select(it => it.Value).Sum())
        .Should()
        .Be(expected);
  }

  public interface IInstruction
  {
    void Operate(List<List<WhirlpoolNumber>> grid);
  }

  public class ShiftBy(int Amount, ISelector Selector) : IInstruction
  {
    public void Operate(List<List<WhirlpoolNumber>> grid)
    {
      var originPoints = Selector.Each(grid).ToList();
      List<Point> destinationPoints = [.. originPoints[Amount..], .. originPoints[..Amount]];
      foreach (var (value, destination) in originPoints.Select(q => grid.At(q)).ToList().Zip(destinationPoints))
      {
        grid.Set(destination, value);
      }
    }
  }

  public class MathOp(Func<WhirlpoolNumber, WhirlpoolNumber> op, ISelector selector) : IInstruction
  {
    public void Operate(List<List<WhirlpoolNumber>> grid)
    {
      foreach (var p in selector.Each(grid))
      {
        grid.Set(p, op(grid.At(p)));
      }
    }
  }

  public interface ISelector
  {
    IEnumerable<Point> Each(List<List<WhirlpoolNumber>> grid);
  }

  public class RowSelector(int Row) : ISelector
  {
    public IEnumerable<Point> Each(List<List<WhirlpoolNumber>> grid)
    {
      return Enumerable.Range(0, grid[0].Count).Select(col => new Point(col, Row));
    }
  }

  public class ColSelector(int Col) : ISelector
  {
    public IEnumerable<Point> Each(List<List<WhirlpoolNumber>> grid)
    {
      return Enumerable.Range(0, grid.Count).Select(row => new Point(Col, row));
    }
  }

  public class AllSelector : ISelector
  {
    public IEnumerable<Point> Each(List<List<WhirlpoolNumber>> grid)
    {
      for (var row = 0; row < grid.Count; row++)
      {
        for (var col = 0; col < grid[0].Count; col++)
        {
          yield return new(col, row);
        }
      }
    }
  }

  public record WhirlpoolNumber(long Value)
  {
    public const long Max = 1073741823;
    public static WhirlpoolNumber operator *(WhirlpoolNumber self, long scalar) => Limit(self.Value * scalar);
    public static WhirlpoolNumber operator +(WhirlpoolNumber self, long scalar) => Limit(self.Value + scalar);
    public static WhirlpoolNumber operator -(WhirlpoolNumber self, long scalar) => Limit(self.Value - scalar);
    public static WhirlpoolNumber Limit(long value) {
      if (value > 1073741823) return new(value % (Max + 1));
      while (value < 0) value += (Max + 1);
      return new(value);
    }
  }

  public record World(List<List<WhirlpoolNumber>> Grid, List<IInstruction> Instructions, List<string> Flow);

  private static World GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();

    var selectors =
      (P.Format("COL {}", P.Int).Select(it => new ColSelector(it - 1) as ISelector)) |
      (P.Format("ROW {}", P.Int).Select(it => new RowSelector(it - 1) as ISelector)) |
      (P.String("ALL").Trim().Select(it => new AllSelector() as ISelector));

    var shift = P.Format("SHIFT {} BY {}", selectors, P.Int)
      .Select(it => new ShiftBy(it.Second, it.First) as IInstruction);

    var mul = P.Format("MULTIPLY {} {}", P.Int, selectors).Select(it => new MathOp((a) => checked(a * it.First), it.Second) as IInstruction);
    var add = P.Format("ADD {} {}", P.Int, selectors).Select(it => new MathOp((a) => checked(a + it.First), it.Second) as IInstruction);
    var sub = P.Format("SUB {} {}", P.Int, selectors).Select(it => new MathOp((a) => checked(a - it.First), it.Second) as IInstruction);

    var instruction = shift | mul | add | sub;


    return new(P.Long.Trim().Select(it => new WhirlpoolNumber(it)).Star().ParseMany(pps[0]),
      instruction.ParseMany(pps[1]),
      P.Word.ParseMany(pps[2]));
  }
}
