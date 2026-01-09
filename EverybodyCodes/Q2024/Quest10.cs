using FluentAssertions;
using Mnq.Quest.CSharp.EverybodyCodes;
using Mng.Quest.CSharp.Utils;

namespace Mng.Quest.CSharp.EverybodyCodes.Q2024;

public class Quest10
{
  [Theory]
  [InlineData("Quest10.1.Sample.txt", "PTBVRCZHFLJWGMNS")]
  [InlineData("Quest10.1.txt", "TZWDQJMLVKXNHCBP")]
  public void Part1(string inputFile, string expected)
  {
    var grids = GetInput(inputFile);

    GetRunicWord(grids.Single()).Should().Be(expected);
  }

  [Theory]
  [InlineData("Quest10.1.Sample.txt", 1851)]
  [InlineData("Quest10.2.txt", 190982)]
  public void Part2(string inputFile, long expected)
  {
    var grids = GetInput(inputFile);
    
    grids.Select(GetRunicWord).Sum(RunicPower).Should().Be(expected);
  }

  static long RunicPower(string word)
  {
    return word.Select((c, i) => (c - 'A' + 1) * (i + 1)).Sum();
  }

  static string GetRunicWord(Grid<char> grid)
  {
    string result = "";
    for (var y = 2; y < grid.Height - 2; y++)
    {
      for (var x = 2; x < grid.Width - 2; x++)
      {
        HashSet<char> row = [grid[y, 0], grid[y, 1], grid[y, 6], grid[y, 7]];
        HashSet<char> col = [grid[0, x], grid[1, x], grid[6, x], grid[7, x]];
        row.IntersectWith(col);
        result += $"{row.Single()}";
      }
    }
    return result;
  }


  private static List<Grid<char>> GetInput(string inputFile)
  {
    var masterGrid = ECLoader.ReadLines(inputFile).Gridify();
    List<Grid<char>> result = [];
    for (var y = 0; y < masterGrid.Height; y += 9)
    {
      for (var x = 0; x < masterGrid.Width; x += 9)
      {
        List<List<char>> extracted = [
          masterGrid[y+0][x..(x+8)],
          masterGrid[y+1][x..(x+8)],
          masterGrid[y+2][x..(x+8)],
          masterGrid[y+3][x..(x+8)],
          masterGrid[y+4][x..(x+8)],
          masterGrid[y+5][x..(x+8)],
          masterGrid[y+6][x..(x+8)],
          masterGrid[y+7][x..(x+8)],
        ];
        result.Add(new(extracted));
      }
    }

    return result;
  }

}
