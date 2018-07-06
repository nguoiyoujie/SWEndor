using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class NebulonBATI : WarshipGroup
  {
    private static NebulonBATI _instance;
    public static NebulonBATI Instance()
    {
      if (_instance == null) { _instance = new NebulonBATI(); }
      return _instance;
    }

    private NebulonBATI() : base("Nebulon-B Frigate")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 950.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 32.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 2f;

      Score_perStrength = 15;
      Score_DestroyBonus = 10000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb.x");
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
      ainfo.SetStateS("AddOn_1", "Nebulon B Turbolaser Tower, 60, 90, -520, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_2", "Nebulon B Turbolaser Tower, 0, -180, -550, 90, 0, 0, true");

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 2;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.SelfRegenRate = 0.1f;

      ainfo.CamLocations.Add(new TV_3DVECTOR(66, 78, -480));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 75, 2000));

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

