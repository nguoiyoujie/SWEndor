using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ExplosionGroup : ActorTypeInfo
  {
    internal ExplosionGroup(string name): base(name)
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CullDistance = 12500f;

      RadarSize = 1;
    }
  }
}

