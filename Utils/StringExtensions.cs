namespace Mng.Quest.CSharp.Utils;

public static class StringExtensions
{
    public static List<string> Lines(this string s) => s.Split('\n')
        .Select(it => it.Trim()).ToList();

    public static Grid<char> Gridify(this List<string> self) =>
        new(self);

    public static Grid<T> Gridify<T>(this List<string> self, Func<char, T> xform) =>
        new(self.Select(it => it.Select(xform)));

    public static List<List<string>> Paragraphs(this string self) => self.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
      .Select(l => l.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList()).ToList();
}
