namespace Mng.Quest.CSharp.Utils;

static class PrintGrid
{
    public static void Print(HashSet<Point> points, char on, char off)
    {
        var minx = points.Min(it => it.X);
        var maxx = points.Max(it => it.X);
        var miny = points.Min(it => it.Y);
        var maxy = points.Max(it => it.Y);
        for(var y = miny; y <= maxy; y++)
        {
            for(var x = minx; x <= maxx; x++)
            {
                Console.Write(points.Contains(new(x,y)) ? on : off);
            }
            Console.WriteLine();
        }
    }
}