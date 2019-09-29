﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower03ATI : Groups.SurfaceTower
  {
    internal Tower03ATI(Factory owner) : base(owner, "Radar Tower")
    {
      MaxStrength = 75;
      ImpactDamage = 120;

      ScoreData = new ScoreData(50, 5000);

      RenderData.RadarSize = 2.5f;

      RenderData.AlwaysShowInRadar = true;

      MeshData = new MeshData(Name, @"towers\tower_03.x");
    }
  }
}

