using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ExecutorBridgeATI : AddOnGroup
  {
    private static ExecutorBridgeATI _instance;
    public static ExecutorBridgeATI Instance()
    {
      if (_instance == null) { _instance = new ExecutorBridgeATI(); }
      return _instance;
    }

    private ExecutorBridgeATI() : base("Executor Super Star Destroyer Bridge")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 600.0f;
      ImpactDamage = 200.0f;
      RadarSize = -1;

      CullDistance = 20000;

      Score_perStrength = 75;
      Score_DestroyBonus = 100000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_bridge.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "Explosion";
      ainfo.DeathExplosionSize = 7;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 5;
      ainfo.ExplosionType = "Explosion";
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        List<ActorInfo> parents = ainfo.GetAllParents(1);
        if (parents.Count > 0)
        {
          List<ActorInfo> cs = new List<ActorInfo>(parents[0].GetAllChildren(1));
          foreach (ActorInfo pn in cs)
          {
            if (pn.TypeInfo is ExecutorShieldGeneratorATI)
            {
              ainfo.Strength = ainfo.TypeInfo.MaxStrength;
            }
          }
        }
      }

      if (ainfo.ActorState == ActorState.DYING)
      {
        List<ActorInfo> parents = ainfo.GetAllParents(1);
        if (parents.Count > 0)
        {
          parents[0].Strength *= 0.75f;
        }
      }
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

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ainfo, hitby, impact, normal);
      if (!hitby.TypeInfo.IsDamage)
      {
        if (ainfo.StrengthFrac < 0.5f)
        {
          ainfo.Strength = 0;
          hitby.DestroyedEvents.Clear();
        }
        else
        {
          ainfo.Strength -= hitby.TypeInfo.ImpactDamage * 4;
        }
      }

      bool hasshield = false;
      List<ActorInfo> parents = ainfo.GetAllParents(1);
      if (parents.Count > 0)
      {
        foreach (ActorInfo pn in parents[0].GetAllChildren(1))
        {
          if (pn.TypeInfo is ExecutorShieldGeneratorATI)
          {
            hasshield = true;
          }
        }

        if (!hasshield)
        {
          if (parents[0].StrengthFrac > ainfo.StrengthFrac)
            parents[0].Strength = ainfo.StrengthFrac * parents[0].TypeInfo.MaxStrength;
        }
      }
    }
  }
}

