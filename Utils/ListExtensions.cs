using Mng.Quest.CSharp.Utils;

public static class ListExtensions
{
  public static List<List<T>> Transpose<T>(this List<List<T>> self)
  {
    List<List<T>> result = [];
    for (var c = 0; c < self[0].Count; c++)
    {
      result.Add([.. self.Select(row => row[c])]);
    }
    return result;
  }

  public static List<List<T>> Clone<T>(this List<List<T>> self)
  {
    return self.Select(it => it.ToList()).ToList();
  }

  public static List<List<T>> GridFlipVertical<T>(this List<List<T>> self)
  {
    var o = self.Clone();
    return (o as IEnumerable<List<T>>).Reverse().ToList();
  }

  public static List<List<T>> GridFlipHorizontal<T>(this List<List<T>> self)
  {
    var o = self.Clone();
    return o.Select(row => (row as IEnumerable<T>).Reverse().ToList()).ToList();
  }

  public static List<List<T>> GridRotateRight<T>(this List<List<T>> self)
  {
    // precondition: self has x and y coordiates the same
    var o = self.Clone();
    for (var y = 0; y < self.Count; y++)
    {
      for (var x = 0; x < self.Count; x++)
      {
        var p = new Point(self.Count - y - 1, x);
        o.Set(p, self[y][x]);
      }
    }
    return o;
  }

  public static List<List<T>> GridRotate180<T>(this List<List<T>> self) => self.GridRotateLeft().GridRotateLeft();

  public static List<List<T>> GridRotateLeft<T>(this List<List<T>> self)
  {
    // precondition: self has x and y coordiates the same
    var o = self.Clone();
    for (var y = 0; y < self.Count; y++)
    {
      for (var x = 0; x < self.Count; x++)
      {
        var p = new Point(y, self.Count - x - 1);
        o.Set(p, self[y][x]);
      }
    }
    return o;
  }

  public static T At<T>(this List<List<T>> self, Point p)
  {
    return self[(int)p.Y][(int)p.X];
  }

  public static void Set<T>(this List<List<T>> self, Point p, T value)
  {
    self[(int)p.Y][(int)p.X] = value;
  }
}