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
      ainfo.CombatInfo.CollisionDamageModifier = 0.25f;
      ainfo.ExplosionInfo.ExplosionRate = 0.5f;
      ainfo.ExplosionInfo.ExplosionSize = 5;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        ActorInfo parent = ActorFactory.Get(ainfo.GetTopParent());
        if (parent != null)
        {
          List<int> cs = new List<int>(parent.GetAllChildren(1));
          foreach (int i in cs)
          {
            ActorInfo pn = ActorFactory.Get(i);
            if (pn?.TypeInfo is ExecutorShieldGeneratorATI)
              ainfo.CombatInfo.onNotify(Actors.Components.CombatEventType.RECOVER, ainfo.TypeInfo.MaxStrength);
          }
        }
      }
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState.IsDyingOrDead())
      {
        ainfo.CombatInfo.OnTimedLife = true;
        ainfo.CombatInfo.TimedLife = 2000f;
        ainfo.CombatInfo.IsCombatObject = false;

        ActorInfo parent = ActorFactory.Get(ainfo.GetTopParent());
        if (parent != null)
          parent.ActorState = ActorState.DYING;
      }
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo hitby = ActorFactory.Get(hitbyActorID);
      if (owner == null || hitby == null)
        return;

      if (!hitby.TypeInfo.IsDamage && owner.StrengthFrac < 0.5f)
      {
        owner.ActorState = ActorState.DYING;
        hitby.DestroyedEvents = null;
      }
    }
  }
}

