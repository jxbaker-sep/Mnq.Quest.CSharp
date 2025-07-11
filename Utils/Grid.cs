namespace Mng.Quest.CSharp.Utils;

public class Grid<T>
{
  private readonly List<List<T>> Actual;
  public long Width{get; init;}
  public long Height{get; init;}
  public long Count => Width * Height;

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

  public bool Contains(Point p) => 0 <= p.Y && p.Y < Height && 0 <= p.X && p.X < Width;

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

  public IEnumerable<Point> Points() => MiscUtils.LongRange(0, Height).SelectMany(y => MiscUtils.LongRange(0, Width).Select(x => new Point(x,y)));
  
  public IEnumerable<(Point Key, T Value)> Items() => MiscUtils.LongRange(0, Height).SelectMany(y => MiscUtils.LongRange(0, Width).Select(x => (Key: new Point(x,y), Value: this[new Point(x,y)])));
}