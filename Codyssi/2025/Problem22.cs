using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
using Utils;
using Mng.Quest.CSharp.Utils;
using System.Numerics;
using System.Data;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem22
{
  [Theory]
  [InlineData("Problem22.Sample.1.txt", 3, 3, 5, 146)]
  [InlineData("Problem22.Sample.2.txt", 10, 15, 60, 32545)]
  [InlineData("Problem22.txt", 10, 15, 60, 32422)]
  public void Part1(string inputFile, long maxX, long maxY, long maxZ, int expected)
  {
    var input = GetInput(inputFile);
    input.SelectMany(it => Find(it, [maxX, maxY, maxZ])).Should().HaveCount(expected);
  }

  public static IEnumerable<List<long>> Find(Rule r, List<long> maxXYZ)
  {
    for (long x = 0; x < maxXYZ[0]; x += 1)
    for (long y = 0; y < maxXYZ[1]; y += 1)
    for (long z = 0; z < maxXYZ[2]; z += 1)
    for (long w = -1; w <= 1; w += 1)
    if (MathUtils.MathMod(x * r.Coefficients[0] + y * r.Coefficients[1] + z * r.Coefficients[2] + w * r.Coefficients[3],
      r.Divisor) == r.Remainder) yield return [x, y, z, w];
  }

  public record Rule(List<long> Coefficients, long Divisor, long Remainder, List<long> Velocity);

  private List<Rule> GetInput(string inputFile)
  {
    var p1 = P.Format("{}x+{}y+{}z+{}a", P.Long, P.Long, P.Long, P.Long)
      .Select(it => new List<long> { it.First, it.Second, it.Third, it.Fourth });
    return P.Format("RULE {}: {} DIVIDE {} HAS REMAINDER {} | DEBRIS VELOCITY ({})",
      P.Long, p1, P.Long, P.Long, P.Long.Star(","))
      .Select(it => new Rule(it.Second, it.Third, it.Fourth, it.Fifth))
      .ParseMany(CodyssiLoader.ReadLines(inputFile));

  }
}
