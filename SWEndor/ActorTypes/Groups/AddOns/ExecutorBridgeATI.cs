using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorBridgeATI : Groups.AddOn
  {
    internal ExecutorBridgeATI(Factory owner) : base(owner, "Executor Super Star Destroyer Bridge")
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
      //ainfo.CombatInfo.HitWhileDyingLeadsToDeath = false;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        ActorInfo parent = Owner.Engine.ActorFactory.Get(ainfo.GetTopParent());
        if (parent != null)
        {
          List<int> cs = new List<int>(parent.GetAllChildren(1));
          foreach (int i in cs)
          {
            ActorInfo pn = Owner.Engine.ActorFactory.Get(i);
            if (pn?.TypeInfo is ExecutorShieldGeneratorATI)
              ainfo.CombatInfo.Strength = ainfo.TypeInfo.MaxStrength;
          }
        }
      }

      if (ainfo.ActorState == ActorState.DYING)
      {
        ActorInfo parent = Owner.Engine.ActorFactory.Get(ainfo.GetTopParent());
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

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
      ActorInfo hitby = Owner.Engine.ActorFactory.Get(hitbyActorID);
      if (owner == null || hitby == null)
        return;

      if (!hitby.TypeInfo.IsDamage)
      {
        if (owner.StrengthFrac < 0.5f)
        {
          owner.CombatInfo.Strength = 0;
          hitby.DestroyedEvents = null;
        }
        else
        {
          owner.CombatInfo.Strength -= hitby.TypeInfo.ImpactDamage * 4;
        }
      }

      //bool hasshield = false;
      ActorInfo parent = Owner.Engine.ActorFactory.Get(owner.GetTopParent());
      if (parent != null)
      {
        /*
        foreach (ActorInfo pn in parent.GetAllChildren(1))
          if (pn.TypeInfo.TargetType.HasFlag(TargetType.SHIELDGENERATOR))
            hasshield = true;
            */

        //if (!hasshield)
          if (parent.StrengthFrac > owner.StrengthFrac)
            parent.CombatInfo.Strength = owner.StrengthFrac * parent.TypeInfo.MaxStrength;
      }
    }
  }
}

