using SWEndor.Actors;
using SWEndor.Core;

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

    public override void Process(Engine engine, ActorInfo actor)
    {
      actor.SetSpawnerEnable(Enabled);
      Complete = true;
    }
  }
}
