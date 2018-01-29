using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ArquitensATI : StarDestroyerGroup
  {
    private static ArquitensATI _instance;
    public static ArquitensATI Instance()
    {
      if (_instance == null) { _instance = new ArquitensATI(); }
      return _instance;
    }

    private ArquitensATI() : base("Arquitens Light Cruiser")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 225.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 125.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 8.0f;
      MaxTurnRate = 1.6f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"arquitens\arquitens.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      List<float[]> ttowers = new List<float[]>();

      // Sides
      ainfo.SetStateS("AddOn_0", "Arquitens Turbolaser Tower, 85, 22, 16, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_1", "Arquitens Turbolaser Tower, 40, 22, 170, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_2", "Arquitens Turbolaser Tower, -85, 22, 16, 0, 72, 0, true");
      ainfo.SetStateS("AddOn_3", "Arquitens Turbolaser Tower, -40, 22, 170, 0, 72, 0, true");

      // Top
      ainfo.SetStateS("AddOn_4", "Arquitens Turbolaser Tower, 38, 50, 16, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_5", "Arquitens Turbolaser Tower, -38, 50, 16, 255, 90, 90, true");

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 80, -45));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 80, 2000));

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 1.5f;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, 0);
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

