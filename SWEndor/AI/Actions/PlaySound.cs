using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class PlaySound : ActionInfo
  {
    public PlaySound(string name, bool squadLeaderOnly, bool interrupt = true) : base("PlaySound")
    {
      SoundName = name;
      CanInterrupt = false;
      Interrupt = interrupt;
      SquadLeaderOnly = squadLeaderOnly;
    }

    // parameters
    public string SoundName;
    public bool Interrupt;
    public bool SquadLeaderOnly = false;

    public override string ToString()
    {
      return string.Format("{0},{1}"
                          , Name
                          , SoundName
                          , Interrupt
                          , SquadLeaderOnly
                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (!SquadLeaderOnly || actor.Squad.IsNull || actor.Squad.Leader == actor)
        engine.SoundManager.SetSound(SoundName, Interrupt);
      Complete = true;
    }
  }
}
