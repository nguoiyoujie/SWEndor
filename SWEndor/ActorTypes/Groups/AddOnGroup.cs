using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class AddOnGroup : ActorTypeInfo
  {
    internal AddOnGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CullDistance = 3500f;

      RadarSize = 1;

      Attack_AngularDelta = 360f;
      Attack_HighAccuracyAngularDelta = 360f;

      IsTargetable = true;
      IsHardPointAddon = true;

      NoMove = true;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
    }
  }
}

