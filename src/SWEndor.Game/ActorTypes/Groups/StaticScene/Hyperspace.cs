using MTV3D65;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;

namespace SWEndor.Game.ActorTypes.Instances
{
  internal class Hyperspace : Groups.StaticScene
  {
    internal Hyperspace(Factory owner) : base(owner, "HYPER", "Hyperspace")
    {
      MeshData = new MeshData(Engine, Name, @"special\hyper.x", new float3(15, 15, 15), CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Hyper");
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


