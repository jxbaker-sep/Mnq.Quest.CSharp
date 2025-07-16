using FluentAssertions;

namespace Mng.Quest.CSharp;

public class Problem09: Program
{
  [Theory]
  [InlineData("|||,||||", "|||<||||")]
  [InlineData("||||,|||", "||||>|||")]
  [InlineData("|||,|||", "|||=|||")]
  public void Part1(string input, string expected)
  {
    Dictionary<char, State> finalizers = [];
    foreach(var (c, label) in new[]{('<', "lt"), ('=', "eq"), ('>', "gt")}) {
      var finalizer = CreateState($"finalizer_{label}");
      finalizer.On("|", r => r.Left());
      finalizer.On("<", r => r.Write(c).Then(Halt));
      finalizers[c] = finalizer;
    }

    var previousUp = Init;
    var zeroDown = CreateState("zeroDown");
    var previousDown = zeroDown;
    zeroDown.On("|", r => r.Then(Halt)); // wrote < while going up
    zeroDown.On("_", r => r.Then(finalizers['=']).Left());
    for (var i = 1; i < 256; i++)
    {
      var goingUp = CreateState($"goingUp{i}");
      var goingDown = CreateState($"goingDown{i}");
      previousUp.On("|", r => r.Then(goingUp));
      goingUp.On(",", r => r.Write('<').Then(goingDown));
      goingDown.On("_", r => r.Then(finalizers['>']).Left());
      goingDown.On("|", r => r.Then(previousDown));
      previousDown = goingDown;
      previousUp = goingUp;
    }



    Write("Problem09.rules");

    var result = new LogicMill(Join()).RunToHalt(input);
    result.Result.Should().Be(expected);
  }
}