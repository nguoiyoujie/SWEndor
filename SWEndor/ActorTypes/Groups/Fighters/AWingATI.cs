﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class AWingATI : Groups.RebelWing
  {
    internal AWingATI(Factory owner) : base(owner, "A-Wing")
    {
      MaxStrength = 18;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 55;

      ScoreData = new ScoreData(500, 2500);

      RegenData = new RegenData(false, 0.08f, 0, 0, 0);

      MeshData = new MeshData(Name, @"awing\awing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 2, 20), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      Loadouts = new string[] { "A_WG_TORP", "A_WG_MISL", "A_WG_LASR" };
    }
  }
}

