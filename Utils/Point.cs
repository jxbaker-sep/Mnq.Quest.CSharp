


namespace Mng.Quest.CSharp.Utils;


public record Point(long X, long Y) {
  internal static readonly Point Zero = new(0, 0);

  public static Point operator+(Point point, Vector vector) => point with {Y = point.Y + vector.Y, X = point.X + vector.X};
  public static Point operator-(Point point, Vector vector) => point with {Y = point.Y - vector.Y, X = point.X - vector.X};

  public long ManhattanDistance(Point other) => Math.Abs(X-other.X) + Math.Abs(Y - other.Y);

  public IEnumerable<Point> CardinalNeighbors() => Vector.Cardinals.Select(v => this + v);
  public IEnumerable<Point> InterCardinalNeighbors() => Vector.InterCardinals.Select(v => this + v);
  public IEnumerable<Point> CompassRoseNeighbors() => Vector.CompassRose.Select(v => this + v);

  public IEnumerable<Point> Follow(Vector v)
  {
    var current = this;
    while (true)
    {
      current += v;
      yield return current;
    }
  }

}