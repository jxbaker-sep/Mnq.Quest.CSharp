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

  public static T At<T>(this List<List<T>> self, Point p)
  {
    return self[(int)p.Y][(int)p.X];
  }

  public static void Set<T>(this List<List<T>> self, Point p, T value)
  {
    self[(int)p.Y][(int)p.X] = value;
  }
}