using FluentAssertions;

namespace Mng.Quest.CSharp.Utils;

public record Hex(long X, long Y)
{
  public static Hex Zero {get;} = new(0, 0);

  public static Hex operator+(Hex h, HexVector v) => new(h.X + v.dX, h.Y + v.dY);

  public long HexattanDistance()
  {
    var n = Math.Abs(X) < Math.Abs(Y) ? X : Y;
    var h2 = this + HexVector.NorthEast * -n;
    return Math.Abs(n) + Math.Abs(h2.X + h2.Y);
  }
}

public record HexVector(long dX, long dY)
{
  public static HexVector North {get;} = new(0, 1);
  public static HexVector NorthEast {get;} = new(1, 1);
  public static HexVector NorthWest {get;} = new(-1, 0);
  public static HexVector South {get;} = new(0, -1);
  public static HexVector SouthEast {get;} = new(1, 0);
  public static HexVector SouthWest {get;} = new(-1, -1);

  public static HexVector operator*(HexVector self, long scalar) => new(self.dX * scalar, self.dY * scalar);
}

public class HexTest
{
  [Fact]
  public void HexattanDistanceTest()
  {
    Hex.Zero.HexattanDistance().Should().Be(0);
    (Hex.Zero + HexVector.North).HexattanDistance().Should().Be(1);
    (Hex.Zero + HexVector.NorthEast).HexattanDistance().Should().Be(1);
    (Hex.Zero + HexVector.SouthEast).HexattanDistance().Should().Be(1);
    (Hex.Zero + HexVector.South).HexattanDistance().Should().Be(1);
    (Hex.Zero + HexVector.SouthWest).HexattanDistance().Should().Be(1);
    (Hex.Zero + HexVector.NorthWest).HexattanDistance().Should().Be(1);
    (Hex.Zero + HexVector.North * 2).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.NorthEast * 2).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.SouthEast * 2).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.South * 2).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.SouthWest * 2).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.NorthWest * 2).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.NorthEast * 2 + HexVector.South).HexattanDistance().Should().Be(2);
    (Hex.Zero + HexVector.SouthWest * 2 + HexVector.North).HexattanDistance().Should().Be(2);
  }
}