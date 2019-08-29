using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_LN_ATI : Groups.TIE
  {
    internal TIE_LN_ATI(Factory owner) : base(owner, "TIE")
    {
      MaxStrength = 4;
      ImpactDamage = 16;
      MaxSpeed = 350;
      MinSpeed = 175;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 45;

      Score_perStrength = 400;
      Score_DestroyBonus = 400;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_far.x");

      Cameras = new ActorCameraInfo[]
      {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 12), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("TIE_WingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("TIE_WingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };

      Loadouts = new string[] { "TIE__LASR", "TIE__LASR_AI" };
    }
  }
}

