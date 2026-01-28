using Utils;

namespace Mng.Quest.CSharp.Utils;

public class Grid<T>
{
  private readonly List<List<T>> Actual;
  public int Width { get; init; }
  public int Height { get; init; }
  public long Count => (long)Width * Height;

  public Grid(IEnumerable<IEnumerable<T>> input)
  {
    Actual = input.Select(it => it.ToList()).ToList();
    Height = Actual.Count;
    Width = Actual[0].Count;
  }

  public T this[Point p]
  {
    get => Actual[(int)p.Y][(int)p.X];
    set => Actual[(int)p.Y][(int)p.X] = value;
  }

  public T this[int y, int x]
  {
    get => Actual[y][x];
    set => Actual[y][x] = value;
  }

  public List<T> this[int y]
  {
    get => Actual[y];
  }

  public bool Contains(Point p) => 0 <= p.Y && p.Y < Height && 0 <= p.X && p.X < Width;

  public T Get(Point p, T deflt)
  {
    if (Contains(p)) return this[p];
    return deflt;
  }

  public bool TryGetValue(Point p, out T output)
  {
    if (Contains(p))
    {
      output = this[p];
      return true;
    }
    output = default!;
    return false;
  }

  public IEnumerable<Point> Points() => MiscUtils.LongRange(0, Height).SelectMany(y => MiscUtils.LongRange(0, Width).Select(x => new Point(x, y)));

  public IEnumerable<Point> Points(Func<Point, T, bool> filter) => Items().Where(x => filter(x.Point, x.Value)).Select(it => it.Point);

  public IEnumerable<List<T>> Lines => Actual;

  public IEnumerable<(Point Point, T Value)> Items() => MiscUtils.LongRange(0, Height).SelectMany(y => MiscUtils.LongRange(0, Width).Select(x => (Key: new Point(x, y), Value: this[new Point(x, y)])));

  public string Printable()
  {
    return Actual.Select(it => it.Join()).Join("\n");
  }
}