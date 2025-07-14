using Utils;

public class Program
{
  private readonly Dictionary<(string, char), Rule> Rules = [];

  public class State(string Name, Program Parent)
  {
    public string Name { get; } = Name;
    public Program Parent { get; } = Parent;
    public static bool operator ==(State lhs, State rhs) => lhs.Name == rhs.Name;
    public static bool operator !=(State lhs, State rhs) => !(lhs == rhs);

    public State On(char Token, Func<Rule, Rule> callback)
    {
      var rule = new Rule(this, Token, this, Token, Direction.Right);
      Parent.Rules[(Name, Token)] = callback(rule);
      return this;
    }

    public State On(string Lexicon, Func<Rule, Rule> callback)
    {
      foreach(var token in Lexicon) On(token, callback);
      return this;
    }

    public Rule Skip(char Token)
    {
      var rule = new Rule(this, Token, this, Token, Direction.Right);
      Parent.Rules[(Name, Token)] = rule;
      return rule;
    }

    public void Skip(string Lexicon)
    {
      foreach(var token in Lexicon) Skip(token);
    }

    public override bool Equals(object? obj)
    {
      if (obj is State other) return this == other;
      return false;
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }
  }

  public enum Direction { Left, Right };

  public record Rule(State State, char Token, State NextState, char NextToken, Direction Direction)
  {
    public override string ToString()
    {
      var d = Direction == Direction.Left ? 'L' : 'R';
      return $"{State.Name} {Token} {NextState.Name} {NextToken} {d}";
    }
    public Rule Right() => this with { Direction = Direction.Right };
    public Rule Left() => this with { Direction = Direction.Left };
    public Rule Write(char Token) { 
      if (Token == ' ') throw new ApplicationException("Cannot write token <space>.");
      return this with { NextToken = Token };
    }
    public Rule Write(string Tokens) 
    {
      var rule = Write(Tokens[0]);
      var state = State;
      foreach(var next in Tokens[1..])
      {
        state = state.Parent.CreateState();
        rule.Then(state);
        state.On(Token, r => rule = r.Write(next));
      }
      return rule;
    }
    public Rule Then(State NextState) => this with { NextState = NextState };
    public Rule Then(Action<State> callback) {
      var state = State.Parent.CreateState();
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
      s = IdToStateLabel(NextStateId++);
    }
    else {
      if (s.Any(it => it == ' ')) throw new ApplicationException($"State Label cannot contain spaces: {s}");
    }
    return new State(s, this);
  }

  private static string IdToStateLabel(int id)
  {
    var first = id % 30;
    var l = Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c).Concat("äöõü").ToList();
    string s = "" + l[first];
    while (id >= 30)
    {
      id /= 30;
      first = (id - 1) % 30;
      s += l[first];
    }
    return s;
  }

  public Program()
  {
    Init = new State("INIT", this);
    Halt = new State("HALT", this);
  }

  public State Init { get; private set; }
  public State Halt { get; private set; }
  public const char Blank = '_';
  public const char Bar = '|';

  public string Join(bool compact = false)
  {
    var rules = Rules.Values.ToList();
    if (compact)
    {
      var id = 0;
      var oldRules = rules;
      rules = [];
      Dictionary<String, State> map = [];
      map["INIT"] = Init;
      map["HALT"] = Halt;
      foreach(var rule in oldRules)
      {
        if (!map.TryGetValue(rule.State.Name, out var s1))
        {
          map[rule.State.Name] = new State(IdToStateLabel(id++), this);
        }
        if (!map.TryGetValue(rule.NextState.Name, out var s2))
        {
          map[rule.NextState.Name] = new State(IdToStateLabel(id++), this);
        }
        rules.Add(rule with { State = map[rule.State.Name], NextState = map[rule.NextState.Name] });
      }
    }
    return rules.Select(it => it.ToString()).Join("\n");
  }

  public void Write(string filename, bool compact = true)
  {
    File.WriteAllText($"/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/{filename}", Join(compact));
  }
}