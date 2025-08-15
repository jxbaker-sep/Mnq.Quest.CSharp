using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;
using FluentAssertions;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem20
{
  [Theory]
  [InlineData("Problem20.Sample.1.txt", 3, 201474)]
  [InlineData("Problem20.Sample.2.txt", 80, 6902016000)]
  [InlineData("Problem20.txt", 80, 244785900620800)]
  public void Part1(string inputFile, int size, long expected)
  {
    var world = GetInput(inputFile);

    var cube = new CubeOfGrids(size);
    foreach(var (i, t) in world.Instructions.Zip(world.Twists))
    {
      Apply(cube, i);
      // Console.WriteLine($"{cube.Facing} = {cube.Absorption(cube.Facing)}");
      if (t == 'L') cube.RotateLeft();
      else if (t == 'U') cube.RotateUp();
      else if (t == 'R') cube.RotateRight();
      else if (t == 'D') cube.RotateDown();
      else throw new ApplicationException();
    }
    Apply(cube, world.Instructions[^1]);
    // Console.WriteLine($"{cube.Facing} = {cube.Absorption(cube.Facing)}");

    new[] { 1, -1, 2, -2, 3, -3 }
      .Select(it => cube.Absorption(new Point3(0, it, 0)))
      .OrderByDescending(it => it)
      .Take(2)
      .Product()
      .Should().Be(expected);

    // cube.Print();
  }

  private static void Apply(CubeOfGrids cube, Instruction i)
  {
    var startRow = 0;
    var startCol = 0;
    var stopRow = cube.Size;
    var stopCol = cube.Size;
    var face = cube.Current;

    if (i.Selector == "ROW") {
      startRow = i.Index - 1;
      stopRow = i.Index;
    } else if (i.Selector == "COL") {
      startCol = i.Index - 1;
      stopCol = i.Index;
    }
    
    for (var x = startCol; x < stopCol; x++)
    {
      for (var y = startRow; y < stopRow; y++)
      {
        face[y][x] += i.Value;
      }
    }
  }

  public class CubeOfGrids
  {
    public Point3 Facing { get; private set; } = new(1,2,3);
    private readonly Dictionary<long, List<List<long>>> Faces = [];
    public int Size { get; }

    public CubeOfGrids(int size)
    {
      Faces[Facing.Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
      Faces[Facing.RotateDown().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
      Faces[Facing.RotateUp().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
      Faces[Facing.RotateLeft().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
      Faces[Facing.RotateRight().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
      Faces[Facing.RotateDown().RotateDown().Y] = Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
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
      Facing = Facing.RotateDown();
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
      Console.WriteLine($"Up: {Absorption(Facing)}");
      PrintGrid(Facing);
      Console.WriteLine($"Down: {Absorption(Facing.RotateDown().RotateDown())}");
      PrintGrid(Facing.RotateDown().RotateDown());
      Console.WriteLine($"Left: {Absorption(Facing.RotateRight())}");
      PrintGrid(Facing.RotateRight());
      Console.WriteLine($"Right: {Absorption(Facing.RotateLeft())}");
      PrintGrid(Facing.RotateLeft());
      Console.WriteLine($"Front: {Absorption(Facing.RotateDown())}");
      PrintGrid(Facing.RotateDown());
      Console.WriteLine($"Back: {Absorption(Facing.RotateUp())}");
      PrintGrid(Facing.RotateUp());
    }

    public long Absorption(Point3 facing)
    {
      return Faces[facing.Y].Sum(it => it.Sum()) - Size * Size;
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
    return new(parser.ParseMany(pps[0]), [.. pps[1][0]]);
  }
}
