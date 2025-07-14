using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

class LogicMill
{
  public const int MaxSteps = 1_000_000;

  public string CurrentState { get; private set; } = StartState;
  public int TapePosition { get; private set; } = 0;
  public bool Debug { get; private set; }

  public const char Blank = '_';
  public const string StartState = "INIT";
  public const string HaltState = "HALT";

  public LogicMill(string stringRules)
  {
    if (stringRules.Length > 710_000) throw new ApplicationException($"Too many rules! Max length 710_000, found {stringRules.Length}");

    Rules = ParseTransitionRules([.. stringRules.Split("\n", StringSplitOptions.RemoveEmptyEntries)])
      .ToDictionary(it => (it.CurrentState, it.CurrentSymbol), it => it);

    var states = Rules.Select(it => it.Value.CurrentState).Distinct().ToList();
    if (states.Count > 1024) throw new ApplicationException($"Too many states! Max 1024, found {states.Count}");
  }

  private readonly Dictionary<(string, char),TransitionRule> Rules;

  public (string Result, int Steps) RunToHalt(string tapeAsString)
  {
    var input = tapeAsString.ToList();
    var (result, steps) = RunToHaltOnCharList(input);
    while (result.Count > 0 && result[0] == Blank) result = result[1..];
    while (result.Count > 0 && result[^1] == Blank) result = result[..^1];
    return (result.Join(), steps);
  }

  private (List<char>, int) RunToHaltOnCharList(List<char> tape)
  {
    int steps = 0;
    var temp = tape.WithIndices().Where(it => it.Value != Blank).Select(it => it.Index).Take(1).ToList();
    if (temp.Count == 0) throw new ApplicationException("Tape is blank!");
    TapePosition = temp[0];
    CurrentState = StartState;
    for (; steps < MaxSteps && CurrentState != HaltState; steps++)
    {
      if (Debug)
      {
        Console.WriteLine($"{CurrentState}: {TapeToString(tape)}");
      }
      var rule = Rules[(CurrentState, tape[TapePosition])];
      CurrentState = rule.NewState;
      tape[TapePosition] = rule.NewSymbol;
      TapePosition += rule.MoveDirection;
      if (TapePosition >= tape.Count)
      {
        tape.Add(Blank);
        if (tape.Count > 1_048_576) throw new ApplicationException("Too many cells on tape.");
      };
      if (TapePosition == -1)
      {
        tape = [Blank, .. tape];
        TapePosition += 1;
        if (tape.Count > 1_048_576) throw new ApplicationException("Too many cells on tape.");
      }
    }
    return (tape, steps);

    string TapeToString(List<char> tape)
    {
      return tape.WithIndices().Select(it => it.Index == TapePosition ? $"{it.Value}←" : $"{it.Value}").Join();
    }
  }

  public record TransitionRule(string CurrentState, char CurrentSymbol, string NewState, char NewSymbol, int MoveDirection);
  public static List<TransitionRule> ParseTransitionRules(List<string> input)
  {
    var comment = P.Format("//{}", P.Any.Star().Void()).Optional();
    input = input.Where(it => !string.IsNullOrWhiteSpace(it) && comment.Parse(it).Count == 0).ToList();
    var identifier = P.Sequence(P.Letter, (P.Letter | P.Digit | P.Any.Where(it => it == '_')).Star().Join()).Select(it => it.First + it.Second);
    return P.Format("{} {} {} {} {} {}", identifier, P.Any, identifier, P.Any, P.Choice("L", "R"), comment)
      .Select(it => new TransitionRule(it.First, it.Second, it.Third, it.Fourth, it.Fifth == "L" ? -1 : 1))
      .ParseMany(input);
  }
}

public class LogicMillTests
{
  [Fact]
  public void Sanity()
  {
    var result = new LogicMill(@"
INIT | FIND | R
FIND | FIND | R
FIND _ HALT | R
").RunToHalt("_||||_");
    result.Should().Be(("|||||", 5));
  }

}