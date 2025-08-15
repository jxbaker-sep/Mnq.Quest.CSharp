using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem20
{
  [Theory]
  [InlineData("Problem20.Sample.txt", 12645822)]
  // [InlineData("Problem20.txt", 1148860702)]
  public void Part1(string inputFile, long expected)
  {
    var items = GetInput(inputFile);

    var instruction = new Instruction("FACE", 0, 1);
    var cube = new CubeOfGrids(3);
    Apply(cube, new Instruction("FACE", 0, 1));
    cube.RotateLeft();
    Apply(cube, new Instruction("FACE", 0, 2));
    cube.Print();
  }

  public static void Apply(CubeOfGrids cube, Instruction i)
  {
    if (i.Selector == "FACE")
    {
      var face = cube.Current;
      for (var x = 0; x < cube.Size; x++)
      {
        for (var y = 0; y < cube.Size; y++)
        {
          face[y][x] = (y == x) ? (y * 3 + x) : i.Value;
    }
      }
    }
    else throw new NotImplementedException();
  }

  public class CubeOfGrids
  {
    private Point3 Facing = new(1,2,3);
    private readonly Dictionary<long, List<List<long>>> Faces = [];
    public int Size { get; }

    public CubeOfGrids(int size)
    {
      Faces[Facing.Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(0L, size).ToList()).ToList();
      Faces[Facing.RotateDown().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(0L, size).ToList()).ToList();
      Faces[Facing.RotateUp().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(0L, size).ToList()).ToList();
      Faces[Facing.RotateLeft().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(0L, size).ToList()).ToList();
      Faces[Facing.RotateRight().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(0L, size).ToList()).ToList();
      Faces[Facing.RotateDown().RotateDown().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(0L, size).ToList()).ToList();
      Size = size;
    }

    public List<List<long>> Current => Faces[Facing.Y];

    private void Roll(Point3 facing1, Point3 facing2)
    {
      var o1 = Faces[facing1.Y];
      var o2 = Faces[facing2.Y];
      Faces[facing1.Y] = o1.Transpose();
      Faces[facing2.Y] = o2.Transpose().Select(it => (it as IEnumerable<long>).Reverse().ToList()).ToList();
    }

    public void RotateUp()
    {
      Facing = Facing.RotateUp();
      Roll(Facing.RotateLeft(), Facing.RotateRight());
    }

    public void RotateDown()
    {
      Facing = Facing.RotateUp();
      Roll(Facing.RotateRight(), Facing.RotateLeft());
    }

    public void RotateLeft()
    {
      Facing = Facing.RotateLeft();
      Roll(Facing.RotateUp(), Facing.RotateDown());
    }

    public void RotateRight()
    {
      Facing = Facing.RotateRight();
      Roll(Facing.RotateDown(), Facing.RotateUp());

    }

    public void Print()
    {
      Console.WriteLine($"Facing = {Facing}");
      Console.WriteLine($"Up:");
      PrintGrid(Facing);
      Console.WriteLine($"Down:");
      PrintGrid(Facing.RotateDown().RotateDown());
      Console.WriteLine($"Left:");
      PrintGrid(Facing.RotateRight());
      Console.WriteLine($"Right:");
      PrintGrid(Facing.RotateLeft());
      Console.WriteLine($"Front:");
      PrintGrid(Facing.RotateDown());
      Console.WriteLine($"Back:");
      PrintGrid(Facing.RotateUp());
    }

    public void PrintGrid(Point3 facing)
    {
      var grid = Faces[facing.Y];
      Console.WriteLine(grid.Select(it => it.Join()).Join("\n"));
    }
  }

  public record Instruction(string Selector, int Index, long Value);
  public record World(List<Instruction> Instructions, List<char> Twists);

  private static World GetInput(string inputFile)
  {
    var pps = CodyssiLoader.ReadAllText(inputFile).Paragraphs();
    var parser = P.Format("{} {} - VALUE {}", P.Word, P.Int.Optional(), P.Long)
      .Select(it => new Instruction(it.First, it.Second.Count > 0 ? it.Second[0] : 0, it.Third));
    return new(parser.ParseMany(pps[0]), [.. pps[0][0]]);
  }
}
