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
  public class MissileBoatATI : Groups.TIE
  {
    internal MissileBoatATI(Factory owner) : base(owner, "Missile Boat")
    {
      MaxStrength = 12;
      ImpactDamage = 16;
      MaxSpeed = 475;
      MinSpeed = 175;
      MaxSpeedChangeRate = 250;
      MaxTurnRate = 48;

      ZTilt = 0.75f;
      ZNormFrac = 0.006f;

      Score_perStrength = 800;
      Score_DestroyBonus = 2000;

      RegenData = new RegenData { SelfRegenRate = 0.05f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_vader.x");

      Cameras = new ActorCameraInfo[]
      {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("MIS__TORP") }
                                                        , {"misl", WeaponFactory.Get("MIS__MISL") }
                                                        , {"laser", WeaponFactory.Get("MIS__LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:misl", "2:misl", "1:torp", "2:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:misl", "1:laser" };
    }
  }
}

