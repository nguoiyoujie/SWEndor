using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class AWingATI : Groups.RebelWing
  {
    internal AWingATI(Factory owner) : base(owner, "A-Wing")
    {
      MaxStrength = 18;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 300;
      MaxTurnRate = 55;

      Score_perStrength = 500;
      Score_DestroyBonus = 2500;

      RegenData = new RegenData { SelfRegenRate = 0.08f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"awing\awing.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 2, 20),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 5, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("A_WG_TORP")}
                                                                         , {"misl", WeaponFactory.Get("A_WG_MISL")}
                                                                         , {"laser", WeaponFactory.Get("A_WG_LASR")} 
                                                                         };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:torp", "1:misl" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:misl", "1:laser" };
    }
  }
}

