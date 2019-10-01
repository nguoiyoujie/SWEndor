﻿using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarATI : Groups.StaticScene
  {
    internal DeathStarATI(Factory owner) : base(owner, "DSTAR", "DeathStar")
    {
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

      MeshData = MeshDataDecorator.CreateAlphaTexturedWall(Name, 20000, "deathstar/deathstar.bmp", "deathstar/deathstaralpha.bmp");
    }

    public override void Dying(Engine engine, ActorInfo ainfo)
    {
      base.Dying(engine, ainfo);
      ainfo.DyingTimerSet(5, true);
    }
  }
}


