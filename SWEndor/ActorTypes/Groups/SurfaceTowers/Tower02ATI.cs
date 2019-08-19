﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower02ATI : Groups.SurfaceTower
  {
    internal Tower02ATI(Factory owner) : base(owner, "Gun Tower")
    {
      MaxStrength = 25;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02.x");
      AddOns = new AddOnInfo[] { new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(0, 50, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }
  }
}

