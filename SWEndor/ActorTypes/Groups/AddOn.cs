﻿using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class AddOn : ActorTypeInfo
  {
    internal AddOn(Factory owner, string name) : base(owner, name)
    {
      // Combat
      Explodes = new ExplodeInfo[] { new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH) };
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;

      RenderData.CullDistance = 3500;

      RenderData.RadarSize = 1;

      Mask = ComponentMask.STATIC_ACTOR;
      AIData.HuntWeight = 1;
    }
  }
}

