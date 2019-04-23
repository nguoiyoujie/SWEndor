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

    public override void Process(ActorInfo owner)
    {
      owner.SetSpawnerEnable(Enabled);
      Complete = true;
    }
  }
}
