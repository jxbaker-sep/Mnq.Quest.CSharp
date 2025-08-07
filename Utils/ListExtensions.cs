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
}