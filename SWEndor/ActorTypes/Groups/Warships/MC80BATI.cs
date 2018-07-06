using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class MC80BATI : WarshipGroup
  {
    private static MC80BATI _instance;
    public static MC80BATI Instance()
    {
      if (_instance == null) { _instance = new MC80BATI(); }
      return _instance;
    }

    private MC80BATI() : base("Mon Calamari 80B Capital Ship")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 2800.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 30.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 3f;
      ZNormFrac = 0.006f;
      ZTilt = 3.5f;

      Score_perStrength = 20;
      Score_DestroyBonus = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mc80b.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CamDeathCircleRadius = 1500;
      ainfo.CamDeathCircleHeight = 250;
      ainfo.CamDeathCirclePeriod = 30;

      ainfo.SetStateS("AddOn_0", "MC90 Turbolaser Tower, 0, 45, 1200, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_1", "MC90 Turbolaser Tower, -120, 42, 950, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_2", "MC90 Turbolaser Tower, 120, 42, 950, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_3", "MC90 Turbolaser Tower, 180, 48, 520, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_4", "MC90 Turbolaser Tower, -180, 48, 520, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_5", "MC90 Turbolaser Tower, 180, -65, 410, 90, 0, 0, true");
      ainfo.SetStateS("AddOn_6", "MC90 Turbolaser Tower, -180, -65, 410, 90, 0, 0, true");

      ainfo.SetStateS("AddOn_7", "MC90 Turbolaser Tower, 220, 52, 300, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_8", "MC90 Turbolaser Tower, -220, 52, 300, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_9", "MC90 Turbolaser Tower, 210, -75, 150, 90, 0, 0, true");
      ainfo.SetStateS("AddOn_10", "MC90 Turbolaser Tower, -210, -75, 150, 90, 0, 0, true");


      ainfo.SetStateS("AddOn_11", "MC90 Turbolaser Tower, 0, 120, -225, -90, 0, 0, true");

      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 45, 660));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 45, 2000));

      // Generate States
      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 3.5f;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.SelfRegenRate = 0.2f;

      ainfo.Scale *= 1.1f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, -750 - z_displacement);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (!ainfo.IsPlayer())
        {
          if (dist < 1500)
          {
            if (PlayerInfo.Instance().enginelgvol < 1 - dist / 1500.0f)
              PlayerInfo.Instance().enginelgvol = 1 - dist / 1500.0f;
          }
        }
      }
    }
  }
}

