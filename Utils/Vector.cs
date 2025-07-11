

namespace Mng.Quest.CSharp.Utils;


public record Vector(long X, long Y) {
  public static Vector North {get;} = new(0, -1);
  public static Vector East {get;} = new(1, 0);
  public static Vector South {get;} = new(0, 1);
  public static Vector West {get;} = new(-1, 0);

  public static Vector NorthEast {get;} = North + East;
  public static Vector SouthEast {get;} = South + East;
  public static Vector NorthWest {get;} = North + West;
  public static Vector SouthWest {get;} = South + West;

  public static IEnumerable<Vector> Cardinals {get;} = [North, East, South, West];
  public static IEnumerable<Vector> InterCardinals {get;} = [NorthEast, SouthEast, SouthWest, NorthWest];
  public static IEnumerable<Vector> CompassRose {get;} = [North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest];

  public static Vector operator+(Vector v1, Vector v2) => new(v1.X + v2.X, v1.Y + v2.Y);
  public static Vector operator-(Vector v1, Vector v2) => new(v1.X - v2.X, v1.Y - v2.Y);
  public static Vector operator*(Vector v1, long v2) => new(v1.X * v2, v1.Y * v2);

  internal Vector RotateRight() => new(-Y, X);
  internal Vector RotateLeft() => new(Y, -X);
}