using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class Hyperspace : Groups.StaticScene
  {
    internal Hyperspace(Factory owner) : base(owner, "HYPER", "Hyperspace")
    {
      MeshData = new MeshData(Name, @"special\hyper.x", 15, "Hyper");
      TimedLifeData = new TimedLifeData(false, 0.25f);
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      ActorInfo player = engine.PlayerInfo.TempActor;
      if (player != null)
      {
        ActionInfo act = player.CurrentAction;
        if (act is HyperspaceOut)
        {

        }
        else if (act is HyperspaceIn && player.MoveData.Speed < 50000 && !ainfo.IsDyingOrDead)
        {
          ainfo.SetState_Dying();
        }
        else if (!(act is HyperspaceIn) && !ainfo.IsDyingOrDead)
        {
          ainfo.SetState_Dying();
        }
      }
      else
      {
        ainfo.SetState_Dead();
      }
    }
  }
}


