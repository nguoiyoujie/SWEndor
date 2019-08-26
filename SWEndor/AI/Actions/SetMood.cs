using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.AI.Actions
{
  public class SetMood : ActionInfo
  {
    public SetMood(int mood, bool squadLeaderOnly) : base("SetMood")
    {
      Mood = mood;
      CanInterrupt = false;
      SquadLeaderOnly = squadLeaderOnly;
    }

    // parameters
    public int Mood = 0;
    public bool SquadLeaderOnly = false;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , Mood
                          , SquadLeaderOnly
                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (!SquadLeaderOnly || actor.Squad.IsNull || actor.Squad.Leader == actor)
        engine.GameScenarioManager.Scenario.Mood = Mood;
      Complete = true;
    }
  }
}
