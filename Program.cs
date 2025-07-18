using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;
using Microsoft.VisualBasic;
using Utils;
using Xunit.Sdk;

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
      else
      {
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
      Rules.Add(rule);
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
      if (tokens.Length - 1 == index)
      {
        next.On(CurrentToken, r => applyToLast(r.Write(tokens[index])));
        return;
      }
      var nextNext = CurrentState.Parent.CreateState();
      next.On(CurrentToken, r => r.Write(tokens[index]).Then(nextNext));
      WriteRecursive(nextNext, tokens, index + 1, applyToLast);
    }

    public Rule Write(string tokens, Func<Rule, Rule> applyToLast)
    {
      if (tokens.Length == 0) throw new ApplicationException("Can't write 0-length string!");
      if (tokens.Length > 1)
      {
        var next = CurrentState.Parent.CreateState($"Write_{IdToLabel(CurrentState.Parent.NextStateId++)}");
        var result = Write(tokens[0]).Then(next);
        WriteRecursive(next, tokens, 1, applyToLast);
        return result;
      }
      else
      {
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
  private List<PseudoRule> Optimized = [];

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

  public List<PseudoRule> Optimize() => OptimizedRules(States);

  public class PseudoRule(string CurrentState, char CurrentToken, string NextState, char NextToken, Direction Direction)
  {
    public string CurrentState { get; } = CurrentState;
    public char CurrentToken { get; } = CurrentToken;
    public string NextState { get; set; } = NextState;
    public char NextToken { get; } = NextToken;
    public Direction Direction { get; } = Direction;
  }

  public static List<PseudoRule> OptimizedRules(IReadOnlyList<State> instates)
  {
    var startRules = instates.Sum(it => it.Rules.Count);

    List<PseudoRule> rules = [];
    Dictionary<string, List<int>> states = [];
    foreach(var instate in instates.Where(state => state.Rules.Count > 0))
    {
      List<int> myRules = [];
      foreach(var rule in instate.Rules)
      {
        rules.Add(new PseudoRule(rule.CurrentState.Label, rule.CurrentToken, rule.NextState.Label, rule.NextToken, rule.Direction));
        myRules.Add(rules.Count - 1);
      }
      states[instate.Label] = myRules;
    }

    var init = "INIT";

    // Remove unreferenced states
    Dictionary<string, List<int>> references = states.Values.SelectMany(it => it).Select(id => (rule: rules[id], id)).Where(it => it.rule.CurrentState != it.rule.NextState)
      .GroupToDictionary(it => it.rule.NextState, it => it.id);

    if (references.TryGetValue(init, out var initFound)) initFound.Add(-1);
    else references[init] = [-1];

    while (true)
    {
      // Console.WriteLine("Unreferenced " + states.Count);
      var removed = states.Keys.Where(state => !references.TryGetValue(state, out var found) || found.Count == 0).ToList();
      if (removed.Count == 0) break;
      foreach(var state in removed) {
        foreach(var rn in states[state]) {
          if (references.TryGetValue(rules[rn].NextState, out var needle)) needle.Remove(rn);
        }
        states.Remove(state);
      }
    }

    // Combine similar states
    // states are the same if they have all the same rules
    string CreateKey(string s) => states[s].Select(rn => rules[rn])
      .OrderBy(it => it.CurrentToken)
      .Select(r => $"{r.CurrentToken} {r.NextState} {r.NextToken} {r.Direction}")
      .Join(", ");
    var keyToStates = states.Keys
      .GroupToDictionary(CreateKey, it => it)
      .ToDictionary(it => it.Key, it => it.Value.ToHashSet());
    while (true)
    {
      // Console.WriteLine("Similar " + states.Count);
      var first = keyToStates.Where(it => it.Value.Count > 1).Take(1).Select(it => it.Value).ToList();
      if (first.Count == 0) break;
      var group = first[0];
      var me = group.Contains(init) ? init : group.First();
      var remainder = group.Except([me]).ToList();
      var key = CreateKey(me);
      foreach(var state in remainder)
      {
        foreach(var rn in (references.GetValueOrDefault(state) ?? []).ToList())
        {
          var rule = rules[rn];
          var parent = rule.CurrentState;

          var oldKey = CreateKey(parent);
          keyToStates[oldKey].Remove(parent);

          rules[rn].NextState = me;
          var newKey = CreateKey(parent);
          if (keyToStates.TryGetValue(newKey, out HashSet<string>? value)) {
            value.Add(parent);
          } else {
            keyToStates[newKey] = [parent];
          }

          // references should not contain rules where Current == Next
        }
        keyToStates[key].Remove(state);
        references.Remove(state);
        states.Remove(state);
      }
    }    

    var endRules = states.Values.Sum(it => it.Count);
    Console.WriteLine($"Optimized away {startRules - endRules} rules. Returning {endRules}");

    return states.Values.SelectMany(it => it).Select(it => rules[it]).ToList();
  }

  public string Join(bool compact = false)
  {
    if (Optimized.Count == 0)
    {
      Optimized = OptimizedRules(States);
    }
    var rules = Optimized;
    // var rules = States.SelectMany(it => it.Rules.Select(r => new PseudoRule(r.CurrentState.Label, r.CurrentToken, r.NextState.Label, r.NextToken, r.Direction))).ToList();
    if (compact)
    {
      var id = 0;
      var oldRules = rules;
      rules = [];
      Dictionary<string, string> map = [];
      map["INIT"] = "INIT";
      map["HALT"] = "HALT";
      foreach (var rule in oldRules)
      {
        if (!map.TryGetValue(rule.CurrentState, out var s1))
        {
          map[rule.CurrentState] = IdToLabel(id++);
        }
        if (!map.TryGetValue(rule.NextState, out var s2))
        {
          map[rule.NextState] = IdToLabel(id++);
        }
        rules.Add(new PseudoRule(map[rule.CurrentState], rule.CurrentToken, map[rule.NextState], rule.NextToken, rule.Direction));
      }
    }
    return rules.Select(it =>
    {
      var dir = it.Direction == Direction.Left ? 'L' : 'R';
      return $"{it.CurrentState} {it.CurrentToken} {it.NextState} {it.NextToken} {dir}";
  
    }).Join("\n");
  }

  public void Write(string filename, bool compact = true)
  {
    File.WriteAllText($"/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/{filename}", Join(compact));
  }
}