namespace Mng.Quest.CSharp.Utils;

public record Point3(long X, long Y, long Z)
{
  internal static readonly Point3 Zero = new(0, 0, 0);

  public static Point3 operator +(Point3 point, Vector3 vector) => new(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);
  public static Point3 operator -(Point3 point, Vector3 vector) => new(point.X - vector.X, point.Y - vector.Y, point.Z - vector.Z);

}

public static class Point3Extensions
{
  public static Point3 RotateRight(this Point3 self) => new(self.Y, -self.X, self.Z);
  public static Point3 RotateLeft(this Point3 self) => new(-self.Y, self.X, self.Z);
  public static Point3 RotateUp(this Point3 self) => new(self.X, -self.Z, self.Y);
  public static Point3 RotateDown(this Point3 self) => new(self.X, self.Z, -self.Y);

  public static long ManhattanDistance(this Point3 self, Point3 other) =>
    Math.Abs(self.X - other.X) + Math.Abs(self.Y - other.Y) + Math.Abs(self.Z - other.Z);

  public static double StraightLineDistance(this Point3 self, Point3 other) =>
    Math.Sqrt(Math.Pow(self.X - other.X, 2) + Math.Pow(self.Y - other.Y, 2) + Math.Pow(self.Z - other.Z, 2));

  public static long PseudoStraightLineDistance(this Point3 self, Point3 other) =>
    (self.X - other.X) * (self.X - other.X) + (self.Y - other.Y) * (self.Y - other.Y) + (self.Z - other.Z) * (self.Z - other.Z);
}

public record Vector3(long X, long Y, long Z)
{
  public static Vector3 North { get; } = new(0, -1, 0);
  public static Vector3 East { get; } = new(1, 0, 0);
  public static Vector3 South { get; } = new(0, 1, 0);
  public static Vector3 West { get; } = new(-1, 0, 0);
  public static Vector3 Up { get; } = new(0, 0, 1);
  public static Vector3 Down { get; } = new(0, 0, -1);
  public static Vector3 operator*(Vector3 vector, long scalar) => new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
}