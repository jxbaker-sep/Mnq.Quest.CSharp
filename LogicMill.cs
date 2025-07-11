using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

class LogicMill(string stringRules)
{
  public const int MaxSteps = 1_000_000;

  public string CurrentState { get; private set; } = StartState;
  public int TapePosition { get; private set; } = 0;

  public const char Blank = '_';
  public const string StartState = "INIT";
  public const string HaltState = "HALT";


  private readonly Dictionary<string, Dictionary<char, TransitionRule>> Rules = ParseTransitionRules([.. stringRules.Split("\n", StringSplitOptions.RemoveEmptyEntries)])
    .GroupToDictionary(it => it.CurrentState, it => it)
    .ToDictionary(it => it.Key, it => it.Value.ToDictionary(it => it.CurrentSymbol, it => it));

  public (string Result, int Steps) RunToHalt(string tapeAsString)
  {
    var input = tapeAsString.ToList();
    var (result, steps) = RunToHaltOnCharList(input);
    while (result[0] == Blank) result = result[1..];
    while (result[^1] == Blank) result = result[..^1];
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
      var rule = Rules[CurrentState][tape[TapePosition]];
      CurrentState = rule.NewState;
      tape[TapePosition] = rule.NewSymbol;
      TapePosition += rule.MoveDirection;
      if (TapePosition >= tape.Count) tape.Add(Blank);
      if (TapePosition == -1) { tape = [Blank, .. tape]; TapePosition += 1; }
    }
    return (tape, steps);
  }

  public record TransitionRule(string CurrentState, char CurrentSymbol, string NewState, char NewSymbol, int MoveDirection);
  public static List<TransitionRule> ParseTransitionRules(List<string> input)
  {
    var identifier = P.Sequence(P.Letter, (P.Letter | P.Digit | P.Any.Where(it => it == '_')).Star().Join()).Select(it => it.First + it.Second);
    return P.Format("{} {} {} {} {}", identifier, P.Any, identifier, P.Any, P.Choice("L", "R"))
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