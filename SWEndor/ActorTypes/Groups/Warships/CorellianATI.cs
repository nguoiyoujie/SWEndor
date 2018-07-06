using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class CorellianATI : WarshipGroup
  {
    private static CorellianATI _instance;
    public static CorellianATI Instance()
    {
      if (_instance == null) { _instance = new CorellianATI(); }
      return _instance;
    }

    private CorellianATI() : base("Corellian Corvette")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 575.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 100.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 50.0f;
      MaxTurnRate = 9f;

      ZTilt = 17.5f;
      ZNormFrac = 0.065f;

      Score_perStrength = 10;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"corellian\corellian.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 55, -35));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 55, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 300, -800));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 2;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.SetStateS("AddOn_0", "Corellian Turbolaser Tower, -40, 15, 110, -90, 0, 10, true");
      ainfo.SetStateS("AddOn_1", "Corellian Turbolaser Tower, -40, 15, 80, -90, 0, 10, true");
      ainfo.SetStateS("AddOn_2", "Corellian Turbolaser Tower, 40, 15, 110, -90, 0, -10, true");
      ainfo.SetStateS("AddOn_3", "Corellian Turbolaser Tower, 40, 15, 80, -90, 0, -10, true");
      //bottom
      ainfo.SetStateS("AddOn_4", "Corellian Turbolaser Tower, 0, -45, 150, 90, 0, 0, true");
      ainfo.SetStateS("AddOn_5", "Corellian Turbolaser Tower, 0, 45, 150, -90, 0, 0, true");

      //ainfo.Scale *= 1.5f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, -200 - z_displacement);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (!ainfo.IsPlayer())
        {
          if (dist < 500)
          {
            if (PlayerInfo.Instance().enginelgvol < 1 - dist / 500.0f)
              PlayerInfo.Instance().enginelgvol = 1 - dist / 500.0f;
          }
        }
      }
    }
  }
}

