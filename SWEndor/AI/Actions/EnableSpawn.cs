using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class EnableSpawn : ActionInfo
  {
    public EnableSpawn(bool enabled) : base("Enable Spawn")
    {
      Enabled = enabled;
      CanInterrupt = false;
    }

    private bool Enabled;

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      actor.SetSpawnerEnable(Enabled);
      Complete = true;
    }
  }
}
