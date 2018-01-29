using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Tower01ATI : SurfaceTowerGroup
  {
    private static Tower01ATI _instance;
    public static Tower01ATI Instance()
    {
      if (_instance == null) { _instance = new Tower01ATI(); }
      return _instance;
    }

    private Tower01ATI() : base("Deflector Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 100;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_01.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
      ainfo.SetStateS("AddOn_0", "Advanced Turbolaser Turret, 0, 135, 0, 0, 0, 0, true");

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
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower01_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -72, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

