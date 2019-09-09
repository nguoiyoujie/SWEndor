﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower04ATI : Groups.SurfaceTower
  {
    internal Tower04ATI(Factory owner) : base(owner, "Super Deflector Tower")
    {
      MaxStrength = 100;
      ImpactDamage = 120;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Name, @"towers\tower_01.x");
      AddOns = new AddOnData[] { new AddOnData("Super Turbolaser Turret", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }
  }
}

