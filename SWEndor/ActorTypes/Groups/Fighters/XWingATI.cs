using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class XWingATI : Groups.RebelWing
  {
    internal XWingATI(Factory owner) : base(owner, "X-Wing")
    {
      MaxStrength = 30;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 500;
      MaxTurnRate = 50;

      Score_perStrength = 500;
      Score_DestroyBonus = 2500;

      RegenData = new RegenInfo { SelfRegenRate = 0.08f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
       };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("XWing_RU_LD_WingATI", new TV_3DVECTOR(-30, -30, 0), 0, 2000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RU_LD_WingATI", new TV_3DVECTOR(30, 30, 0), -2000, 0, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RD_LU_WingATI", new TV_3DVECTOR(30, -30, 0), 0, 2000, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RD_LU_WingATI", new TV_3DVECTOR(-30, 30, 0), -2000, 0, 0, 3000, -2500, 2500, 0.5f)
        };

      Loadouts = new string[] { "X_WG_TORP", "X_WG_LASR" };
    }
  }
}

