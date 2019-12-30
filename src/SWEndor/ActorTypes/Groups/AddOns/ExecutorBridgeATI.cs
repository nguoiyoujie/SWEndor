using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class ExecutorBridgeATI : Groups.AddOn
  {
    internal ExecutorBridgeATI(Factory owner) : base(owner, "EXECBRID", "Executor Super Star Destroyer Bridge")
    {
      CombatData = CombatData.DefaultShip;
      ArmorData.Data.Put(DamageType.TORPEDO, 1.25f);
      ArmorData.Data.Put(DamageType.COLLISION, 4);

      ExplodeSystemData.Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL00", 1.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 300;
      SystemData.MaxHull = 300;
      CombatData.ImpactDamage = 200.0f;
      RenderData.RadarSize = -1;
      RenderData.CullDistance = 50000;
      TimedLifeData = new TimedLifeData(false, 2000);

      ScoreData = new ScoreData(75, 100000);

      AIData.TargetType = TargetType.ADDON;
      RenderData.RadarType = RadarType.NULL;

      MeshData = new MeshData(Engine, Name, @"executor\executor_bridge.x");
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);

      if (ainfo.Active)
      {
        ActorInfo parent = ainfo.TopParent;
        if (parent != null)
        {
          foreach (ActorInfo pn in parent.Children)
          {
            if (pn?.TypeInfo is ExecutorShieldGeneratorATI)
              ainfo.InflictDamage(-ainfo.MaxHP, DamageType.ALWAYS_100PERCENT);
          }
        }
      }
    }

    public override void Dying(Engine engine, ActorInfo ainfo)
    {
      base.Dying(engine, ainfo);
      ainfo.TopParent?.SetState_Dying();
    }

    public override void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(engine, owner, hitby, impact, normal);
      if (owner == null || hitby == null)
        return;

      if (!hitby.TypeInfo.AIData.TargetType.Has(TargetType.MUNITION) && owner.HP_Frac < 0.5f)
      {
        owner.SetState_Dying();
        hitby.DestroyedEvents = null;
      }
    }
  }
}

