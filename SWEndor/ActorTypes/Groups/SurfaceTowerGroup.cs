using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class SurfaceTowerGroup : ActorTypeInfo
  {
    internal SurfaceTowerGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CullDistance = 10000;

      RadarSize = 1.5f;

      IsTargetable = true;
      //IsFighter = true;
      //IsShip = true;
      IsHardPointAddon = true;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
    }
  }
}

