using Xunit.Sdk;

namespace Utils;

public static class EnumerableExtensions
{
  public static long Product(this IEnumerable<long> self) => self.Aggregate(1L, (a,b) => a * b);
  public static int Product(this IEnumerable<int> self) => self.Aggregate(1, (a,b) => a * b);

  public static string Join<T>(this IEnumerable<T> objects, string joiner = "") => string.Join(joiner, objects);

  public static IEnumerable<List<T>> Windows<T>(this IEnumerable<T> self, int size)
  {
    var q = new Queue<T>();
    foreach (var item in self)
    {
      q.Enqueue(item);
      if (q.Count > size) q.Dequeue();
      if (q.Count == size) yield return q.ToList();
    }
  }

  public static T? FirstOrNull<T>(this IEnumerable<T> self, Func<T, bool> filter) where T:struct
  {
    foreach(var it in self.Where(filter)) return it;
    return null;
  }

  public static IEnumerable<(T First, T Second)> Pairs<T>(this List<T> self)
  {
    if (self.Count < 2) yield break;
    for (var a = 0; a < self.Count - 1; a++)
    {
      for (var b = a + 1; b < self.Count; b++)
      {
        yield return (self[a], self[b]);
      }
    }
  }

  public static List<T> RemoveCommon<T>(this IReadOnlyList<T> self, IReadOnlyList<T> other)
  {
    var result = self.ToList();
    foreach(var item in other)
    {
      result.Remove(item);
    }
    return result;
  }

  public static IEnumerable<long> OrderedIntersect(this IEnumerable<long> self, IEnumerable<long> other) {
    var lhs = self.GetEnumerator();
    var rhs = other.GetEnumerator();
    if (!lhs.MoveNext()) yield break;
    if (!rhs.MoveNext()) yield break;

    while (true) {
      if (lhs.Current < rhs.Current) {if (!lhs.MoveNext()) yield break; continue;}
      if (lhs.Current > rhs.Current) {if (!rhs.MoveNext()) yield break; continue;}
      if (lhs.Current == rhs.Current) {
        yield return lhs.Current;
        if (!lhs.MoveNext()) yield break;
        if (!rhs.MoveNext()) yield break;
        continue;
      }
    }
  }

  public static Queue<T> ToQueue<T>(this IEnumerable<T> self)
  {
    Queue<T> result = [];
    foreach(var item in self) result.Enqueue(item);
    return result;
  }

  public static IEnumerable<(T Value, int Index)> WithIndices<T>(this IEnumerable<T> self) => self.Select((it, index) => (it, index));

}