using P = Parser.ParserBuiltins;
using Parser;
using FluentAssertions;
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

  [Theory]
  [InlineData("Problem19.Sample.txt", "ozNxANO-pYNonIG-MUantNm-lOSlxki-SDJtdpa-JSXfNAJ")]
  [InlineData("Problem19.txt", "hpqtovk-dRluceF-tsKfPZy-SfPcQfV-nOmLFZg-psWTuPA-gCppIei-bRfwRKE-JnsgGBC-BaSObRq-GhTUiRr-dWmHaDK-XcAWrhv-CqrQVvm")]
  public void Part2(string inputFile, string expected)
  {
    var items = GetInput(inputFile)[..^2];

    var root = new Tree(items[0]);
    foreach(var item in items[1..]) root.Add(item);
    var it = root.Add(new Artifact("missing", 500000));
    (Parents(it) as IEnumerable<string>).Reverse().Join("-").Should().Be(expected);
  }

  List<string> Parents(Tree node)
  {
    Tree? it = node;
    List<string> path = [];
    it = it.Parent;
    while (it != null)
    {
      path.Add(it.Code);
      it = it.Parent;
    }
    return path;
  }

  Tree Find(Tree root, long id)
  {
    if (id == root.Id) return root;
    else if (id < root.Id) return Find(root.Left ?? throw new ApplicationException(), id);
    else return Find(root.Right ?? throw new ApplicationException(), id);
  }

  [Theory]
  [InlineData("Problem19.Sample.txt", "pYNonIG")]
  [InlineData("Problem19.txt", "YdnzjKz")] // JCSIQDT incorrect
  public void Part3(string inputFile, string expected)
  {
    var items = GetInput(inputFile)[..^2];
    var checks = GetInput(inputFile)[^2..];

    var root = new Tree(items[0]);
    foreach(var item in items[1..]) root.Add(item);
    var p1 = Parents(Find(root, checks[0].Id)).ToHashSet();
    var p2 = Parents(Find(root, checks[1].Id));
    p2.First(it => p1.Contains(it)).Should().Be(expected);
  }

  class Tree(Artifact Node, Tree? Parent = null)
  {
    private Artifact Node { get; } = Node;
    public Tree? Parent { get; } = Parent;
    public long Id { get; } = Node.Id;
    public string Code { get; } = Node.Code;
    public Tree? Left { get; private set; } = null;
    public Tree? Right { get; private set; } = null;

    public Tree Add(Artifact item)
    {
      var hand = item.Id < Node.Id;
      if (item.Id < Node.Id)
      {
        if (Left is {} l) return l.Add(item);
        else {
          Left = new(item, this);
          return Left;
        }
      }
      else {
        if (Right is {} r) return r.Add(item);
        else {
          Right = new(item, this);
          return Right;
        }
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
