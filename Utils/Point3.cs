namespace Mng.Quest.CSharp.Utils;

public record Point3(long X, long Y, long Z);

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
}