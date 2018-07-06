using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class NebulonB2ATI : WarshipGroup
  {
    private static NebulonB2ATI _instance;
    public static NebulonB2ATI Instance()
    {
      if (_instance == null) { _instance = new NebulonB2ATI(); }
      return _instance;
    }

    private NebulonB2ATI() : base("Nebulon-B2 Frigate")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 1350.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 36.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 10.0f;
      MaxTurnRate = 2.5f;

      Score_perStrength = 15;
      Score_DestroyBonus = 10000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb2.x");
      /*
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Name);
        SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb.x"), true);
        SourceMesh.Enable(false);
      }
      */
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.SetStateS("AddOn_0", "Nebulon B Turbolaser Tower, 0, 40, 220, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_1", "Nebulon B Turbolaser Tower, 0, 95, -500, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_2", "Nebulon B Turbolaser Tower, 0, -145, -400, 90, 0, 0, true");

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 2;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.SelfRegenRate = 0.1f;

      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 120, -300));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 120, 2000));

      //ainfo.Scale *= 1.5f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 100, -300 - z_displacement);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (PlayerInfo.Instance().Actor != ainfo)
        {
          if (dist < 1000)
          {
            if (PlayerInfo.Instance().enginelgvol < 1 - dist / 1500.0f)
              PlayerInfo.Instance().enginelgvol = 1 - dist / 1500.0f;
          }
        }
      }
    }
  }
}

