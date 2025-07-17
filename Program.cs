using Utils;

namespace Mng.Quest.CSharp;

public class Program
{
  private readonly List<State> States = [];

  public class State(string Name, Program Parent)
  {
    public List<Rule> Rules = [];
    public string Label { get; } = Name;
    public Program Parent { get; } = Parent;
    public static bool operator ==(State lhs, State rhs) => lhs.Label == rhs.Label;
    public static bool operator !=(State lhs, State rhs) => !(lhs == rhs);

    public State On(char token, Func<Rule, Rule> callback, bool replace = false)
    {
      if (!replace)
      {
        var found = Rules.Any(it => it.CurrentToken == token);
        if (found) throw new ApplicationException($"Duplicate rule {(Label, token)}");
      }
      else{
        Rules.RemoveAll(it => it.CurrentToken == token);
      }
      var rule = callback(new Rule(this, token, this, token, Direction.Right));
      Rules.Add(rule);
      return this;
    }

    public State On(string Lexicon, Func<Rule, Rule> callback)
    {
      foreach (var token in Lexicon) On(token, callback);
      return this;
    }

    public Rule Skip(char token)
    {
      var rule = new Rule(this, token, this, token, Direction.Right);
      Rules[token] = rule;
      return rule;
    }

    public void Skip(string Lexicon)
    {
      foreach (var token in Lexicon) Skip(token);
    }

    public override bool Equals(object? obj)
    {
      if (obj is State other) return this == other;
      return false;
    }

    public override int GetHashCode()
    {
      return Label.GetHashCode();
    }
  }

  public enum Direction { Left, Right };

  public record Rule(State CurrentState, char CurrentToken, State NextState, char NextToken, Direction Direction)
  {
    public (string, char) Key => (CurrentState.Label, CurrentToken);

    public override string ToString()
    {
      var d = Direction == Direction.Left ? 'L' : 'R';
      return $"{CurrentState.Label} {CurrentToken} {NextState.Label} {NextToken} {d}";
    }
    public Rule Right() => this with { Direction = Direction.Right };
    public Rule Left() => this with { Direction = Direction.Left };
    public Rule Write(char Token)
    {
      if (Token == ' ') throw new ApplicationException("Cannot write token <space>.");
      return this with { NextToken = Token };
    }

    private void WriteRecursive(State next, string tokens, int index, Func<Rule, Rule> applyToLast)
    {
      if (tokens.Length == 0) throw new ApplicationException();
      if (tokens.Length - 1 == index) {
        next.On(CurrentToken, r => applyToLast(r.Write(tokens[index])));
        return;
      }
      var nextNext = CurrentState.Parent.CreateState();
      next.On(CurrentToken, r => r.Write(tokens[index]).Then(nextNext));
      WriteRecursive(nextNext, tokens, index + 1, applyToLast);
    }

    public Rule Write(string tokens, Func<Rule, Rule> applyToLast)
    {
      Console.WriteLine($"Write {tokens.Length}");

      if (tokens.Length == 0) throw new ApplicationException("Can't write 0-length string!");
      if (tokens.Length > 1) {
        var next = CurrentState.Parent.CreateState();
        var result = Write(tokens[0]).Then(next);
        WriteRecursive(next, tokens, 1, applyToLast);
        return result;
      }
      else {
        return applyToLast(Write(tokens[0]));
      }
    }
    public Rule Then(State NextState) => this with { NextState = NextState };
    public Rule Then(Action<State> callback)
    {
      var state = CurrentState.Parent.CreateState();
      callback(state);
      return this with { NextState = state };
    }
  }

  int NextStateId = 0;

  public State CreateState(string name = "")
  {
    var s = name;
    if (s == "")
    {
      s = IdToLabel(NextStateId++);
    }
    else
    {
      if (s.Any(it => it == ' ')) throw new ApplicationException($"State Label cannot contain spaces: {s}");
      if (States.Any(state => state.Label == s)) throw new ApplicationException($"Already contains state {s}");
    }
    var state = new State(s, this);
    States.Add(state);
    return state;
  }

  private static string IdToLabel(int id)
  {
    var firsts = "abcdefghijklmnopqrstuvwxyzABäöõü".ToList();
    var latters = "abcdefghijklmnopqrstuvwxyzABäöõü0123456789".ToList();
    var first = id % firsts.Count;
    string s = "" + firsts[first];
    id /= firsts.Count;
    while (id > 0)
    {
      first = (id - 1) % latters.Count;
      s += latters[first];
      id /= latters.Count;
    }
    return s;
  }

  public Program()
  {
    Init = CreateState("INIT");
    Halt = CreateState("HALT");
  }

  public State Init { get; private set; }
  public State Halt { get; private set; }
  public const char Blank = '_';
  public const char Bar = '|';

  public void Optimize()
  {
    // Remove States with no rules
    States.RemoveAll(state => state.Rules.Count == 0);
    Console.WriteLine("Entering optimize");
    // Remove unreferenced states
    var tailStatesCount = States.SelectMany(state => state.Rules).Where(rule => rule.CurrentState != rule.NextState).Select(it => it.NextState).GroupToCounts();
    foreach(var state in States) if (!tailStatesCount.ContainsKey(state)) tailStatesCount[state] = 0;
    while (true)
    {
      var zeroes = tailStatesCount.Where(it => it.Value == 0 && it.Key != Init).Select(it => it.Key).ToList();
      Console.WriteLine($"Removed {zeroes.Count} rules:");
      if (zeroes.Count == 0) break;
      foreach (var k in zeroes) {
        tailStatesCount.Remove(k);
        foreach(var r in k.Rules.Where(rule => rule.CurrentState != rule.NextState)) tailStatesCount[r.NextState] -= 1;
        States.Remove(k);
      }
    }


    // Combine similar states
    // states are the same if they have all the same rules
    // original = [.. Rules.Values];
    // List<Rule> current = [.. Rules.Values];

    // static string CreateKey(List<Rule> rules) => rules.OrderBy(it => it.CurrentToken).Select(it => $"{it.CurrentToken} {it.NextState.Label} {it.NextToken} {it.Direction}").Join(",");
    // while (true) {
    //   var stateToRules = current.GroupToDictionary(it => it.CurrentState, it => it);
    //   var byKey = stateToRules.GroupToDictionary(it => CreateKey(it.Value), it => it.Key);
    //   foreach(var k in byKey.Keys.ToList()) byKey[k] = byKey[k].Distinct().ToList();
    //   var sameStatesMaybe = byKey.Values.Where(it => it.Count > 1).Take(1).FirstOrDefault();
    //   if (sameStatesMaybe is {} sameStates) {
    //     var me = sameStates[0];
    //     var remainder = sameStates[1..];
    //     // make sure kept rule is "Init"
    //     if (sameStates.Any(it => it == Init))
    //     {
    //       me = Init;
    //       remainder = sameStates.Where(it => it != Init).ToList();
    //     }
    //     Console.WriteLine($"Combining states {me.Label} and {remainder.Select(it => it.Label).Join(", ")}");
    //     current = current.Where(rule => !remainder.Contains(rule.CurrentState)).ToList();
    //     current = current.Select(rule => rule with { NextState = remainder.Contains(rule.NextState) ? me : rule.NextState }).ToList();
    //     continue;
    //   }
    //   break;
    // }

    // Rules.Clear();
    // foreach (var rule in current)
    // {
    //   Rules[rule.Key] = rule;
    // }
  }

  public string Join(bool compact = false)
  {
    Optimize();
    var rules = States.SelectMany(rules => rules.Rules).ToList();
    if (compact)
    {
      var id = 0;
      var oldRules = rules;
      rules = [];
      Dictionary<string, State> map = [];
      map["INIT"] = Init;
      map["HALT"] = Halt;
      foreach (var rule in oldRules)
      {
        if (!map.TryGetValue(rule.CurrentState.Label, out var s1))
        {
          map[rule.CurrentState.Label] = new State(IdToLabel(id++), this);
        }
        if (!map.TryGetValue(rule.NextState.Label, out var s2))
        {
          map[rule.NextState.Label] = new State(IdToLabel(id++), this);
        }
        rules.Add(rule with { CurrentState = map[rule.CurrentState.Label], NextState = map[rule.NextState.Label] });
      }
    }
    return rules.Select(it => it.ToString()).Join("\n");
  }

  public void Write(string filename, bool compact = true)
  {
    File.WriteAllText($"/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/{filename}", Join(compact));
  }
}