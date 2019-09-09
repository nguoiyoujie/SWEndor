﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class XWingATI : Groups.RebelWing
  {
    internal XWingATI(Factory owner) : base(owner, "X-Wing")
    {
      MaxStrength = 30;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 500;
      MoveLimitData.MaxTurnRate = 50;

      ScoreData = new ScoreData(500, 2500);

      RegenData = new RegenData(false, 0.08f, 0, 0, 0);

      MeshData = new MeshData(Name, @"xwing\xwing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
       };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("XWing_RU_LD_WingATI", new TV_3DVECTOR(-30, -30, 0), 0, 20, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("XWing_RU_LD_WingATI", new TV_3DVECTOR(30, 30, 0), -20, 0, -30, 0, -25, 25, 0.5f),
        new DebrisSpawnerData("XWing_RD_LU_WingATI", new TV_3DVECTOR(30, -30, 0), 0, 20, -30, 0, -25, 25, 0.5f),
        new DebrisSpawnerData("XWing_RD_LU_WingATI", new TV_3DVECTOR(-30, 30, 0), -20, 0, 0, 30, -25, 25, 0.5f)
        };

      Loadouts = new string[] { "X_WG_TORP", "X_WG_LASR" };
    }
  }
}

