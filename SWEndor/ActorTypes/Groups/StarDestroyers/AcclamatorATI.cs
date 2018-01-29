using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class AcclamatorATI : StarDestroyerGroup
  {
    private static AcclamatorATI _instance;
    public static AcclamatorATI Instance()
    {
      if (_instance == null) { _instance = new AcclamatorATI(); }
      return _instance;
    }

    private AcclamatorATI() : base("Acclamator Assault Ship")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 450.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 110.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 8.0f;
      MaxTurnRate = 1.6f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"acclamator\acclamator.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      List<float[]> ttowers = new List<float[]>();

      // Sides
      ainfo.SetStateS("AddOn_0", "Acclamator Turbolaser Tower, 99, 10, 460, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_1", "Acclamator Turbolaser Tower, 148, 10, 360, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_2", "Acclamator Turbolaser Tower, 196, 10, 260, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_3", "Acclamator Turbolaser Tower, -99, 10, 460, 0, 72, 0, true");
      ainfo.SetStateS("AddOn_4", "Acclamator Turbolaser Tower, -148, 10, 360, 0, 72, 0, true");
      ainfo.SetStateS("AddOn_5", "Acclamator Turbolaser Tower, -196, 10, 260, 0, 72, 0, true");

      // Front
      ainfo.SetStateS("AddOn_6", "Acclamator Turbolaser Tower, 0, 10, 610, 90, 0, 0, true");

      // Top
      ainfo.SetStateS("AddOn_7", "Acclamator Turbolaser Tower, 145, 40, 70, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_8", "Acclamator Turbolaser Tower, -145, 40, 70, 255, 90, 90, true");

      // Bottom
      //ainfo.SetStateS("AddOn_9", "Acclamator Turbolaser Tower, -140, -1, 320, 0, 0, 180, true");
      //ainfo.SetStateS("AddOn_10", "Acclamator Turbolaser Tower, 140, -1, 320, 0, 0, 180, true");

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 143, 65));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 143, 2000));

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

