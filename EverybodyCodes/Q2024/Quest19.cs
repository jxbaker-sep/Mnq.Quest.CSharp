using System.Diagnostics;
using System.Text.RegularExpressions;
using FluentAssertions;
using Mng.Quest.CSharp.Utils;
using Mnq.Quest.CSharp.EverybodyCodes;
using Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public partial class Quest19
{
  [Theory]
  [InlineData("Quest19.1.Sample.txt", "WIN")]
  [InlineData("Quest19.1.txt", "3451799363427263")]
  public void Part1(string inputFile, string expected)
  {
    var (commands, grid) = GetInput(inputFile);

    Decode(commands, grid).Should().Be(expected);
  }

  string Decode(string commands, Grid<char> grid)
  {
    var ll = new LinkedList<char>(commands);
    var node = ll.First!;
    for (var y = 1; y < grid.Height - 1; y++)
    {
      for (var x = 1; x < grid.Width - 1; x++)
      {
        if (node.Value == 'R') RotateRight(grid, y, x);
        else RotateLeft(grid, y, x);
        node = node.NextWrapped();
      }
    }

    return grid.Lines.Where(it => it.Contains('<') && it.Contains('>'))
      .Select(it =>
      {
        var match = MyRegex().Match(it.Join());
        return match.Groups[1].Value;
      })
      .First();
  }

  record Direction(int Dx, int Dy);

  readonly Direction Above = new(0, -1);
  readonly Direction AboveRight = new(1, -1);
  readonly Direction BesideRight = new(1, 0);
  readonly Direction BelowRight = new(1, 1);
  readonly Direction Below = new(0, 1);
  readonly Direction BelowLeft = new(-1, 1);
  readonly Direction BesideLeft = new(-1, 0);
  readonly Direction AboveLeft = new(-1, -1);

  void RotateRight(Grid<char> grid, int y, int x)
  {
    var temp = grid[y + Above.Dy][x + Above.Dx];
    List<Direction> sequence = [AboveRight, BesideRight, BelowRight, Below, BelowLeft, BesideLeft, AboveLeft, Above];
    foreach (var (dx, dy) in sequence)
    {
      (temp, grid[y + dy][x + dx]) = (grid[y + dy][x + dx], temp);
    }
  }

  void RotateLeft(Grid<char> grid, int y, int x)
  {
    var temp = grid[y + Above.Dy][x + Above.Dx];
    List<Direction> sequence = [AboveLeft, BesideLeft, BelowLeft, Below, BelowRight, BesideRight, AboveRight, Above];
    foreach (var (dx, dy) in sequence)
    {
      (temp, grid[y + dy][x + dx]) = (grid[y + dy][x + dx], temp);
    }
  }

  private static (string commands, Grid<char> grid) GetInput(string inputFile)
  {
    var lines = ECLoader.ReadLines(inputFile);
    return (lines[0], lines.Skip(2).ToList().Gridify());
  }

  [GeneratedRegex(@".*>(.*?)<.*")]
  private static partial Regex MyRegex();
}
