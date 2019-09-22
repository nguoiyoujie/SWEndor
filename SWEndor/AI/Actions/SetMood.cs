using SWEndor.Actors;
using SWEndor.Scenarios;

namespace SWEndor.AI.Actions
{
  public class SetMood : ActionInfo
  {
    public SetMood(MoodStates mood, bool squadLeaderOnly) : base("SetMood")
    {
      Mood = mood;
      CanInterrupt = false;
      SquadLeaderOnly = squadLeaderOnly;
    }

    // parameters
    public MoodStates Mood = 0;
    public bool SquadLeaderOnly = false;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Mood.ToString()
        , SquadLeaderOnly.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (!SquadLeaderOnly || actor.Squad.IsNull || actor.Squad.Leader == actor)
        engine.GameScenarioManager.Scenario.Mood = Mood;
      Complete = true;
    }
  }
}
