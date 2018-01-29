using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ExecutorATI : StarDestroyerGroup
  {
    private static ExecutorATI _instance;
    public static ExecutorATI Instance()
    {
      if (_instance == null) { _instance = new ExecutorATI(); }
      return _instance;
    }

    private ExecutorATI() : base("Executor Super Star Destroyer")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 1500.0f;
      ImpactDamage = 120.0f;
      MaxSpeed = 30;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 2.0f;
      MaxTurnRate = 2f;

      Score_perStrength = 75;
      Score_DestroyBonus = 100000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CamDeathCircleRadius = 3000;
      ainfo.CamDeathCircleHeight = 600;
      ainfo.CamDeathCirclePeriod = 40;

      ainfo.SetStateS("AddOn_0", "Executor Super Star Destroyer Turbolaser Tower, 500, 55, -450, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_1", "Executor Super Star Destroyer Turbolaser Tower, -500, 55, -450, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_2", "Executor Super Star Destroyer Turbolaser Tower, 570, 35, -700, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_3", "Executor Super Star Destroyer Turbolaser Tower, -570, 35, -700, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_4", "Executor Super Star Destroyer Turbolaser Tower, 200, 125, -600, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_5", "Executor Super Star Destroyer Turbolaser Tower, -200, 125, -600, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_6", "Executor Super Star Destroyer Turbolaser Tower, 200, 125, -800, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_7", "Executor Super Star Destroyer Turbolaser Tower, -200, 125, -800, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_8", "Executor Super Star Destroyer Turbolaser Tower, 200, 140, -415, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_9", "Executor Super Star Destroyer Turbolaser Tower, -200, 140, -415, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_10", "Executor Super Star Destroyer Turbolaser Tower, 350, 150, -840, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_11", "Executor Super Star Destroyer Turbolaser Tower, -350, 150, -840, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_12", "Executor Super Star Destroyer Turbolaser Tower, 70, 125, -60, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_13", "Executor Super Star Destroyer Turbolaser Tower, -70, 125, -60, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_14", "Executor Super Star Destroyer Turbolaser Tower, 200, 130, 95, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_15", "Executor Super Star Destroyer Turbolaser Tower, -200, 130, 95, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_16", "Executor Super Star Destroyer Turbolaser Tower, 215, 70, 550, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_17", "Executor Super Star Destroyer Turbolaser Tower, -215, 70, 550, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_18", "Executor Super Star Destroyer Turbolaser Tower, 215, 70, 675, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_19", "Executor Super Star Destroyer Turbolaser Tower, -215, 70, 675, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_20", "Executor Super Star Destroyer Turbolaser Tower, 340, 50, 410, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_21", "Executor Super Star Destroyer Turbolaser Tower, -340, 50, 410, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_22", "Executor Super Star Destroyer Turbolaser Tower, 75, 85, 1020, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_23", "Executor Super Star Destroyer Turbolaser Tower, -75, 85, 1020, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_24", "Executor Super Star Destroyer Turbolaser Tower, 75, 70, 1160, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_25", "Executor Super Star Destroyer Turbolaser Tower, -75, 70, 1160, -90, 0, 0, true");

      ainfo.SetStateS("AddOn_26", "Executor Super Star Destroyer Turbolaser Tower, 200, 60, 1000, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_27", "Executor Super Star Destroyer Turbolaser Tower, -200, 60, 1000, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_28", "Executor Super Star Destroyer Turbolaser Tower, 200, 65, 1140, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_29", "Executor Super Star Destroyer Turbolaser Tower, -200, 65, 1140, -90, 0, 0, true");

      //Shield Generators
      ainfo.SetStateS("AddOn_30", "Executor Super Star Destroyer Shield Generator, 72, 375, -870, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_31", "Executor Super Star Destroyer Shield Generator, -72, 375, -870, -90, 0, 0, true");

      //Bridge
      ainfo.SetStateS("AddOn_32", "Executor Super Star Destroyer Bridge, 0, 325, -845, 0, 0, 0, true");

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 325, -840));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 325, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 1000, -2000));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 325, 2000));

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 5;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.Scale *= 1.5f;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.OnTimedLife = true;
        ainfo.TimedLife = 2000f;
        ainfo.IsCombatObject = false;
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, -800 - z_displacement);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (PlayerInfo.Instance().Actor != ainfo)
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

