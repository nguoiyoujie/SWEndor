using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class SurfaceVentATI : SurfaceTowerGroup
  {
    private static SurfaceVentATI _instance;
    public static SurfaceVentATI Instance()
    {
      if (_instance == null) { _instance = new SurfaceVentATI(); }
      return _instance;
    }

    private SurfaceVentATI() : base("Thermal Exhaust Port")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 12000;
      ImpactDamage = 120;

      RadarSize = 2.5f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface_vent.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
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
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower00_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -72, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ainfo, hitby, impact, normal);

      if (hitby.TypeInfo is MissileATI)
      {
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}

