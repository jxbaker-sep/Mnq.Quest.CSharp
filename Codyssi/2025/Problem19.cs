using P = Parser.ParserBuiltins;
using Parser;
using Mng.Quest.CSharp.Utils;
using FluentAssertions;
using System.Net;
using Utils;

namespace Mnq.Quest.CSharp.Codyssi;

public class Problem19
{
  [Theory]
  [InlineData("Problem19.Sample.txt", 12645822)]
  [InlineData("Problem19.txt", 1148860702)]
  public void Part1(string inputFile, long expected)
  {
    var items = GetInput(inputFile)[..^2];

    var root = new Tree(items[0]);
    foreach(var item in items[1..]) root.Add(item);
    List<long> layerSums = [];
    Queue<(Tree node, int layer)> open = [];
    open.Enqueue((root, 0));
    while (open.TryDequeue(out var current))
    {
      if (current.layer == layerSums.Count) layerSums.Add(0);
      layerSums[current.layer] += current.node.Id;
      if (current.node.Left is {} l) open.Enqueue((l, current.layer + 1));
      if (current.node.Right is {} r) open.Enqueue((r, current.layer + 1));
    }

    (layerSums.Max() * layerSums.Count).Should().Be(expected);
  }

  class Tree(Artifact Node, Tree? Left = null, Tree? Right = null)
  {
    private Artifact Node { get; } = Node;
    public long Id { get; } = Node.Id;
    public string Code { get; } = Node.Code;
    public Tree? Left { get; private set; } = Left;
    public Tree? Right { get; private set; } = Right;

    public void Add(Artifact item)
    {
      var hand = item.Id < Node.Id;
      if (item.Id < Node.Id)
      {
        if (Left is {} l) l.Add(item);
        else Left = new(item);
      }
      else {
        if (Right is {} r) r.Add(item);
        else Right = new(item);
      }
    }
  }

  record Artifact(string Code, long Id);

  private static List<Artifact> GetInput(string inputFile)
  {
    return P.Format("{} | {}",P.Word, P.Long)
      .Select(it => new Artifact(it.First, it.Second))
      .ParseMany(CodyssiLoader.ReadLines(inputFile).Where(it => it != "").ToList());
  }
}
