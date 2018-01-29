using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class DebrisGroup : ActorTypeInfo
  {
    internal DebrisGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      CullDistance = 4500f;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DamageModifier = 0;
      ainfo.EnableExplosions = true;
      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "Explosion";
    }
  }
}

