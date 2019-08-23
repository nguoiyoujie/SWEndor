using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorBridgeATI : Groups.AddOn
  {
    internal ExecutorBridgeATI(Factory owner) : base(owner, "Executor Super Star Destroyer Bridge")
    {
      CombatData = CombatData.DefaultShip;
      Armor = new ActorInfo.ArmorModel() { Light = 1, Hull = 4};
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

      if (ainfo.Active)
      {
        ActorInfo parent = ainfo.TopParent;
        if (parent != null)
        {
          foreach (ActorInfo pn in parent.Children)
          {
            if (pn?.TypeInfo is ExecutorShieldGeneratorATI)
              ainfo.InflictDamage(ainfo, -ainfo.MaxHP, DamageType.ALWAYS_100PERCENT);
          }
        }
      }
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      if (ainfo.IsDyingOrDead)
      {
        ainfo.DyingTimer.Set(2000, true);
        CombatSystem.Deactivate(Engine, ainfo);

        ainfo.TopParent?.SetState_Dying();
      }
    }

    public override void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(owner, hitby, impact, normal);
      if (owner == null || hitby == null)
        return;

      if (!Engine.MaskDataSet[hitby].Has(ComponentMask.IS_DAMAGE) && owner.HP_Frac < 0.5f)
      {
        owner.SetState_Dying();
        hitby.DestroyedEvents = null;
      }
    }
  }
}

