using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.AI.Actions;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Instances
{
  public class Hyperspace : Groups.StaticScene
  {
    internal Hyperspace(Factory owner) : base(owner, "HYPER", "Hyperspace")
    {
      MeshData = new MeshData(Name, @"special\hyper.x", 15, "Hyper");
      TimedLifeData = new TimedLifeData(false, 1);
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      ActorInfo player = engine.PlayerInfo.TempActor;
      if (player != null)
      {
        ActionInfo act = player.CurrentAction;
        if (!(act is HyperspaceOut))
        {
          if (act is HyperspaceIn && !ainfo.IsDyingOrDead)
          {
            HyperspaceIn h = (HyperspaceIn)act;
            if (h.distance < HyperspaceIn.Max_Speed)
              ainfo.SetState_Dying();
          }
          else if (!(act is HyperspaceIn) && !ainfo.IsDyingOrDead)
          {
            ainfo.SetState_Dying();
          }
        }
      }
      else
      {
        ainfo.SetState_Dead();
      }
    }
  }
}


