public record Point3(long X, long Y, long Z);

public static class Point3Extensions
{
  public static Point3 RotateRight(this Point3 self) => new(self.Y, -self.X, self.Z);
  public static Point3 RotateLeft(this Point3 self) => new(-self.Y, self.X, self.Z);
  public static Point3 RotateUp(this Point3 self) => new(self.X, -self.Z, self.Y);
  public static Point3 RotateDown(this Point3 self) => new(self.X, self.Z, -self.Y);
}