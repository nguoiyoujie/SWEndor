﻿using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class SurfaceTower : ActorTypeInfo
  {
    internal SurfaceTower(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      DyingMoveData.Kill();

      RenderData.CullDistance = 15000;
      RenderData.RadarSize = 1.5f;

      AIData.TargetType = TargetType.ADDON | TargetType.STRUCTURE;
      RenderData.RadarType = RadarType.FILLED_SQUARE;

      ArmorData.Data.Put(DamageType.LIGHT, 1.2f);
      ArmorData.Data.Put(DamageType.HEAVY, 1.2f);

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

