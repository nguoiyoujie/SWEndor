using MTV3D65;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
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

      CullDistance = 30000;

      Score_perStrength = 75;
      Score_DestroyBonus = 100000;

      TargetType = TargetType.ADDON;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_bridge.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.ExplosionInfo.ExplosionRate = 0.5f;
      ainfo.ExplosionInfo.ExplosionSize = 5;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        ActorInfo parent = ainfo.GetTopParent();
        if (parent != null)
        {
          List<ActorInfo> cs = new List<ActorInfo>(parent.GetAllChildren(1));
          foreach (ActorInfo pn in cs)
          {
            if (pn.TypeInfo is ExecutorShieldGeneratorATI)
            {
              ainfo.CombatInfo.Strength = ainfo.TypeInfo.MaxStrength;
            }
          }
        }
      }

      if (ainfo.ActorState == ActorState.DYING)
      {
        ActorInfo parent = ainfo.GetTopParent();
        if (parent != null)
        {
          parent.CombatInfo.Strength *= 0.75f;
        }
      }
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.CombatInfo.OnTimedLife = true;
        ainfo.CombatInfo.TimedLife = 2000f;
        ainfo.CombatInfo.IsCombatObject = false;
      }
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ainfo, hitby, impact, normal);
      if (!hitby.TypeInfo.IsDamage)
      {
        if (ainfo.StrengthFrac < 0.5f)
        {
          ainfo.CombatInfo.Strength = 0;
          hitby.DestroyedEvents.Clear();
        }
        else
        {
          ainfo.CombatInfo.Strength -= hitby.TypeInfo.ImpactDamage * 4;
        }
      }

      //bool hasshield = false;
      ActorInfo parent = ainfo.GetTopParent();
      if (parent != null)
      {
        /*
        foreach (ActorInfo pn in parent.GetAllChildren(1))
          if (pn.TypeInfo.TargetType.HasFlag(TargetType.SHIELDGENERATOR))
            hasshield = true;
            */

        //if (!hasshield)
          if (parent.StrengthFrac > ainfo.StrengthFrac)
            parent.CombatInfo.Strength = ainfo.StrengthFrac * parent.TypeInfo.MaxStrength;
      }
    }
  }
}

