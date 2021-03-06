﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.AI.Actions;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Instances
{
  internal class Hyperspace : Groups.StaticScene
  {
    internal Hyperspace(Factory owner) : base(owner, "HYPER", "Hyperspace")
    {
      MeshData = new MeshData(Engine, Name, @"special\hyper.x", 15, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Hyper");
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
            if (h.distance < HyperspaceIn.Max_Speed * 0.75f)
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


