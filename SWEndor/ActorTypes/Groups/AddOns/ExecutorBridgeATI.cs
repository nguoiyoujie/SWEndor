using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorBridgeATI : Groups.AddOn
  {
    internal ExecutorBridgeATI(Factory owner) : base(owner, "Executor Super Star Destroyer Bridge")
    {
      CombatData = new CombatData(true, false, 1, 4);
      ExplodeData = new ExplodeData(0.5f, 5);

      MaxStrength = 600.0f;
      ImpactDamage = 200.0f;
      RadarSize = -1;

      CullDistance = 30000;

      Score_perStrength = 75;
      Score_DestroyBonus = 100000;

      TargetType = TargetType.ADDON;
      RadarType = RadarType.NULL;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_bridge.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        ActorInfo parent = ActorFactory.Get(ainfo.TopParent);
        if (parent != null)
        {
          List<int> cs = new List<int>(parent.Children);
          foreach (int i in cs)
          {
            ActorInfo pn = ActorFactory.Get(i);
            if (pn?.TypeInfo is ExecutorShieldGeneratorATI)
              CombatSystem.onNotify(Engine, ainfo.ID, Actors.Components.CombatEventType.RECOVER, ainfo.TypeInfo.MaxStrength);
          }
        }
      }
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState.IsDyingOrDead())
      {
        TimedLifeSystem.Activate(Engine, ainfo.ID, 2000f);
        CombatSystem.Deactivate(Engine, ainfo.ID);

        ActorInfo parent = ActorFactory.Get(ainfo.TopParent);
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

      if (!Engine.MaskDataSet[hitbyActorID].Has(ComponentMask.IS_DAMAGE) && Engine.SysDataSet.StrengthFrac_get(ownerActorID) < 0.5f)
      {
        owner.ActorState = ActorState.DYING;
        hitby.DestroyedEvents = null;
      }
    }
  }
}

