using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using Utils;
using FluentAssertions;
using System.Numerics;

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
    var cube = Run(size, world);

    new[] { 1, -1, 2, -2, 3, -3 }
      .Select(face => cube.Absorption[face])
      .OrderByDescending(it => it)
      .Take(2)
      .Product()
      .Should().Be(expected);
  }

  private static CubeOfGrids Run(int size, World world, bool part3 = false)
  {
    var cube = new CubeOfGrids(size);
    foreach (var (i, t) in world.Instructions.Zip(world.Twists))
    {
      ApplyInstruction(cube.Facing.Y, cube, i, part3);
      ApplyTwist(cube, t);
    }
    ApplyInstruction(cube.Facing.Y, cube, world.Instructions[^1], part3);
    return cube;
  }

  private static void ApplyTwist(CubeOfGrids cube, char twist)
  {
    if (twist == 'L') cube.RotateLeft();
    else if (twist == 'U') cube.RotateUp();
    else if (twist == 'R') cube.RotateRight();
    else if (twist == 'D') cube.RotateDown();
    else throw new ApplicationException();
  }

  [Theory]
  [InlineData("UD", "1234")]
  [InlineData("DU", "1234")]
  [InlineData("LR", "1234")]
  [InlineData("RL", "1234")]
  [InlineData("UUUU", "1234")]
  [InlineData("DDDD", "1234")]
  [InlineData("RRRR", "1234")]
  [InlineData("LLLL", "1234")]
  [InlineData("LLUU", "3412")]
  [InlineData("UULL", "3412")]
  [InlineData("DDLL", "3412")]
  [InlineData("DDRR", "3412")]
  [InlineData("RUL", "4123")]
  [InlineData("RDL", "2341")]
  [InlineData("DRU", "4123")]
  [InlineData("URD", "2341")]
  public void Sanity(string twists, string expected)
  {
    var cube = new CubeOfGrids(3);
    cube.Current[0][0] = 0;
    cube.Current[0][1] = 1;
    cube.Current[0][2] = 0;
    cube.Current[1][0] = 4;
    cube.Current[1][1] = 0;
    cube.Current[1][2] = 2;
    cube.Current[2][0] = 0;
    cube.Current[2][1] = 3;
    cube.Current[2][2] = 0;
    foreach (var t in twists)
    {
      ApplyTwist(cube, t);
    }
    $"{cube.Current[0][1]}{cube.Current[1][2]}{cube.Current[2][1]}{cube.Current[1][0]}".Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem20.Sample.1.txt", 3, "118727856")]
  [InlineData("Problem20.Sample.2.txt", 80, "369594451623936000000")]
  [InlineData("Problem20.txt", 80, "70948207997314582072320")]
  public void Part2(string inputFile, int size, string rep_expected)
  {
    var expected = BigInteger.Parse(rep_expected);
    var world = GetInput(inputFile);
    var cube = Run(size, world);

    BigInteger result = new(1);
    foreach (var item in new[] { 1, -1, 2, -2, 3, -3 }
      .Select(it => cube.Dominant(it)))
    {
      result *= item;
    }
    result.Should().Be(expected);
  }

  [Theory]
  [InlineData("Problem20.Sample.1.txt", 3, "59477096746944")]
  [InlineData("Problem20.Sample.2.txt", 80, "118479211258970523303936")]
  [InlineData("Problem20.txt", 80, "13462100238630197295009")]
  public void Part3(string inputFile, int size, string rep_expected)
  {
    var expected = BigInteger.Parse(rep_expected);
    var world = GetInput(inputFile);
    var cube = Run(size, world, true);

    BigInteger result = new(1);
    foreach (var item in new[] { 1, -1, 2, -2, 3, -3 }
      .Select(it => cube.Dominant(it)))
    {
      result *= item;
    }
    result.Should().Be(expected);
  }

  private static void ApplyPart3Instruction(CubeOfGrids cube, Instruction i)
  {
    if (i.Selector == "COL")
    {
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateUp();
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateUp();
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateUp();
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateUp();
    }
    else if (i.Selector == "ROW")
    {
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateLeft();
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateLeft();
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateLeft();
      ApplyInstruction(cube.Facing.Y, cube, i, false);
      cube.RotateLeft();
    }
    else if (i.Selector == "FACE")
    {
      ApplyInstruction(cube.Facing.Y, cube, i, false);
    }
    else throw new ApplicationException();
  }

  private static void ApplyInstruction(long face, CubeOfGrids cube, Instruction i, bool part3)
  {
    if (part3)
    {
      ApplyPart3Instruction(cube, i);
      return;
    }
    var startRow = 0;
    var startCol = 0;
    var stopRow = cube.Size;
    var stopCol = cube.Size;

    if (i.Selector == "ROW")
    {
      startRow = i.Index - 1;
      stopRow = i.Index;
    }
    else if (i.Selector == "COL")
    {
      startCol = i.Index - 1;
      stopCol = i.Index;
    }

    for (var x = startCol; x < stopCol; x++)
    {
      for (var y = startRow; y < stopRow; y++)
      {
        cube.AddPoint(face, new(x, y), i.Value);
      }
    }

  }

  public class CubeOfGrids
  {
    public Point3 Facing { get; private set; } = new(1, 2, 3);
    private readonly Dictionary<long, List<List<long>>> Faces = [];
    public int Size { get; }
    public readonly Dictionary<long, long> Absorption = new(){
      { 1, 0 },
      { -1, 0 },
      { 2, 0 },
      { -2, 0 },
      { 3, 0 },
      { -3, 0 },
    };

    public void AddPoint(long face, Point point, long increment)
    {
      Absorption[face] += increment;

      var value = Faces[face].At(point) + increment;
      if (value > 100) value -= 100;
      Faces[face].Set(point, value);
    }


    public CubeOfGrids(int size)
    {
      List<List<long>> CreateGrid() => Enumerable.Range(0, size).Select(it => Enumerable.Repeat(1L, size).ToList()).ToList();
      Faces[Facing.Y] = CreateGrid();
      Faces[Facing.RotateDown().Y] = CreateGrid();
      Faces[Facing.RotateUp().Y] = CreateGrid();
      Faces[Facing.RotateLeft().Y] = CreateGrid();
      Faces[Facing.RotateRight().Y] = CreateGrid();
      Faces[Facing.RotateDown().RotateDown().Y] = CreateGrid();
      Size = size;
    }

    public List<List<long>> Current => Faces[Facing.Y];

    private void Roll(Point3 facing1, Point3 facing2)
    {
      Faces[facing1.Y] = Faces[facing1.Y].GridRotateRight();
      Faces[facing2.Y] = Faces[facing2.Y].GridRotateLeft();
    }

    public CubeOfGrids RotateUp()
    {
      Facing = Facing.RotateUp();
      Roll(Facing.RotateLeft(), Facing.RotateRight());
      return this;
    }

    public CubeOfGrids RotateDown()
    {
      Facing = Facing.RotateDown();
      Roll(Facing.RotateRight(), Facing.RotateLeft());
      return this;
    }

    public CubeOfGrids RotateLeft()
    {
      var original = Facing;
      Facing = Facing.RotateLeft();
      Roll(Facing.RotateDown(), Facing.RotateUp());
      Faces[original.Y] = Faces[original.Y].GridRotateLeft();
      Faces[original.RotateLeft().Y] = Faces[original.RotateLeft().Y].GridRotateLeft();
      Faces[original.RotateLeft().RotateLeft().Y] = Faces[original.RotateLeft().RotateLeft().Y].GridRotateLeft();
      Faces[original.RotateLeft().RotateLeft().RotateLeft().Y] = Faces[original.RotateLeft().RotateLeft().RotateLeft().Y].GridRotateLeft();
      return this;
    }

    public CubeOfGrids RotateRight()
    {
      var original = Facing;
      Facing = Facing.RotateRight();
      Roll(Facing.RotateUp(), Facing.RotateDown());
      Faces[original.Y] = Faces[original.Y].GridRotateRight();
      Faces[original.RotateLeft().Y] = Faces[original.RotateLeft().Y].GridRotateRight();
      Faces[original.RotateLeft().RotateLeft().Y] = Faces[original.RotateLeft().RotateLeft().Y].GridRotateRight();
      Faces[original.RotateLeft().RotateLeft().RotateLeft().Y] = Faces[original.RotateLeft().RotateLeft().RotateLeft().Y].GridRotateRight();

      return this;
    }

    public void Print()
    {
      Console.WriteLine($"Facing = {Facing}");
      Console.WriteLine($"Up: {Dominant(Facing.Y)}");
      PrintGrid(Facing);
      Console.WriteLine($"Down: {Dominant(Facing.RotateDown().RotateDown().Y)}");
      PrintGrid(Facing.RotateDown().RotateDown());
      Console.WriteLine($"Left: {Dominant(Facing.RotateRight().Y)}");
      PrintGrid(Facing.RotateRight());
      Console.WriteLine($"Right: {Dominant(Facing.RotateLeft().Y)}");
      PrintGrid(Facing.RotateLeft());
      Console.WriteLine($"Front: {Dominant(Facing.RotateDown().Y)}");
      PrintGrid(Facing.RotateDown());
      Console.WriteLine($"Back: {Dominant(Facing.RotateUp().Y)}");
      PrintGrid(Facing.RotateUp());
    }

    public long Dominant(long facing)
    {
      var grid = Faces[facing];
      return grid.Select(it => it.Sum()).Concat(grid.Transpose().Select(it => it.Sum())).Max();
    }

    public void PrintGrid(Point3 facing)
    {
      var grid = Faces[facing.Y];
      Console.WriteLine(grid.Select(it => it.Join(" ")).Join("\n"));
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
