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

    public Rule On(char Token, Func<Rule, Rule> callback)
    {
      var rule = new Rule(this, Token, this, Token, Direction.Right);
      Parent.Rules[(Name, Token)] = callback(rule);
      return rule;
    }

    public void On(string Lexicon, Func<Rule, Rule> callback)
    {
      foreach(var token in Lexicon) On(token, callback);
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
    public Rule Write(char Token) => this with { NextToken = Token };
    public Rule Write(string Tokens) 
    {
      var rule = Write(Tokens[0]);
      var state = State;
      foreach(var next in Tokens[1..])
      {
        state = state.Parent.CreateState();
        rule.Then(state);
        rule = state.On(Token, r => r.Write(next));
      }
      return rule;
    }
    public Rule Then(State NextState) => this with { NextState = NextState };
  }

  int NextStateId = 0;

  public State CreateState()
  {
    return new State($"S{NextStateId++}", this);
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

  public string Join()
  {
    return Rules.Values.Select(it => it.ToString()).Join("\n");
  }

  public void Write(string filename)
  {
    File.WriteAllText($"/home/jxbaker@net.sep.com/dev/Mnq.Quest.CSharp/{filename}", Join());
  }
}