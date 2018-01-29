using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Tower02ATI : SurfaceTowerGroup
  {
    private static Tower02ATI _instance;
    public static Tower02ATI Instance()
    {
      if (_instance == null) { _instance = new Tower02ATI(); }
      return _instance;
    }

    private Tower02ATI() : base("Gun Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 25;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
      ainfo.SetStateS("AddOn_0", "Turbolaser Turret, 0, 50, 0, 0, 0, 0, true");

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.DeathExplosionSize = 5;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      /*
      if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower02_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -21, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

