using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class EnableSpawn : ActionInfo
  {
    public EnableSpawn(bool enabled) : base("Enable Spawn")
    {
      Enabled = enabled;
      CanInterrupt = false;
    }

    private bool Enabled;

    public override void Process(Engine engine, ActorInfo actor)
    {
      actor.SpawnerInfo.Enabled = Enabled;
      Complete = true;
    }
  }
}
