﻿using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class SurfaceTower : ActorTypeInfo
  {
    internal SurfaceTower(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      DyingMoveData.Kill();

      RenderData.CullDistance = 10000;
      RenderData.RadarSize = 1.5f;

      AIData.TargetType = TargetType.ADDON | TargetType.STRUCTURE;
      RenderData.RadarType = RadarType.FILLED_SQUARE;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

