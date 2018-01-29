using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class StaticSceneGroup : ActorTypeInfo
  {
    internal StaticSceneGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      MaxStrength = 1.0f;
      MaxSpeed = 0.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 0.0f;
      MaxTurnRate = 0.0f;
      EnableDistanceCull = false;
      CollisionEnabled = false;

      Score_perStrength = 0;
      Score_DestroyBonus = 0;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DamageModifier = 0;
    }
  }
}

