﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class DeathStar2ATI : Groups.StaticScene
  {
    int[] texanimframes;

    internal DeathStar2ATI(Factory owner) : base(owner, "DSTAR2", "DeathStar2")
    {
      ExplodeSystemData.Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

      int[] death = new int[] { 1, 10, 15, 16, 20, 21, 22, 23, 27, 28, 30, 31, 34, 35, 36 };

      MeshData = MeshDataDecorator.CreateAlphaTexturedFlickerWall(Engine, Name, 20000, "deathstar/deathstar2.bmp", "deathstar/deathstar2b.jpg", "deathstar/deathstar2alpha.bmp", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, 32, death, ref texanimframes);
      TimedLifeData = new TimedLifeData(false, 5);
    }

    public override void Dying(Engine engine, ActorInfo ainfo)
    {
      base.Dying(engine, ainfo);
      ainfo.DyingTimerSet(5, true);
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      if (ainfo.IsDying)
      {
        int k = texanimframes.Length - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * texanimframes.Length);
        if (k >= 0 && k < texanimframes.Length)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}


