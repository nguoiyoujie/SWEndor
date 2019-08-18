using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorBridgeATI : Groups.AddOn
  {
    internal ExecutorBridgeATI(Factory owner) : base(owner, "Executor Super Star Destroyer Bridge")
    {
      CombatData = new CombatData(true, false, 1, 4);

      Explodes = new ExplodeInfo[] 
      {
        new ExplodeInfo("ExpL00", 0.5f, 5, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH)
      };

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

      if (ainfo.Active)
      {
        if (ainfo.TopParent != null)
        {
          foreach (ActorInfo pn in ainfo.TopParent.Children)
          {
            if (pn?.TypeInfo is ExecutorShieldGeneratorATI)
              ainfo.Health.InflictDamage(ainfo, new DamageInfo<ActorInfo>(ainfo, -ainfo.Health.MaxHP, DamageType.ALWAYS_100PERCENT));
          }
        }
      }
    }

    public override void Dying<A1>(A1 self)
    {
      base.Dying(self);
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ainfo.DyingTimer.Set(2000).Start();
      CombatSystem.Deactivate(Engine, ainfo);

      ainfo.TopParent?.StateModel.MakeDying(ainfo.TopParent);
    }

    public override void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(owner, hitby, impact, normal);
      if (owner == null || hitby == null)
        return;

      if (!hitby.StateModel.ComponentMask.Has(ComponentMask.IS_DAMAGE) && owner.Health.Frac < 0.5f)
      {
        owner.StateModel.MakeDying(owner);
        hitby.DestroyedEvents = null;
      }
    }
  }
}

