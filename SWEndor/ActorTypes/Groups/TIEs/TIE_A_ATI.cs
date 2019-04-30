using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_A_ATI : Groups.TIE
  {
    internal TIE_A_ATI(Factory owner) : base(owner, "TIE Avenger")
    {
      MaxStrength = 15;
      ImpactDamage = 16;
      MaxSpeed = 650;
      MinSpeed = 200;
      MaxSpeedChangeRate = 350;
      MaxTurnRate = 60;

      ZTilt = 1.75f;
      ZNormFrac = 0.005f;

      Score_perStrength = 800;
      Score_DestroyBonus = 2000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_vader.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 0, 20),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.ExplosionInfo.DeathExplosionTrigger = DeathExplosionTrigger.ALWAYS;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.075f;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"misl", WeaponFactory.Get("TIEA_MISL") }
                                                        , {"laser", WeaponFactory.Get("TIEA_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:misl" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:misl", "1:laser" };
    }
  }
}

