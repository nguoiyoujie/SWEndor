using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorShieldGeneratorATI : Groups.AddOn
  {
    internal ExecutorShieldGeneratorATI(Factory owner) : base(owner, "Executor Super Star Destroyer Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 150.0f;
      ImpactDamage = 500.0f;
      RadarSize = 2;

      CullDistance = 30000;

      Score_perStrength = 100;
      Score_DestroyBonus = 5000;

      TargetType |= TargetType.SHIELDGENERATOR;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_energy_pod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.RegenerationInfo.ParentRegenRate = 40f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.1f;
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
      ActorInfo hitby = Owner.Engine.ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (!hitby.TypeInfo.IsDamage)
      {
        owner.CombatInfo.Strength = 0;
      }
      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      if (hitby.TypeInfo.IsDamage)
      {
        foreach (int i in owner.GetAllChildren(1))
        {
          ActorInfo child = Owner.Engine.ActorFactory.Get(i);
          if (child?.TypeInfo is ElectroATI)
          {
            child.CycleInfo.CyclesRemaining = 2.5f / child.TypeInfo.TimedLife;
            return;
          }
        }
        ActorCreationInfo acinfo = new ActorCreationInfo(Owner.Engine.ActorTypeFactory.Get("Electro"));
        acinfo.Position = owner.GetPosition();
        ActorInfo electro = ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
        electro.AddParent(ownerActorID);
        electro.CycleInfo.CyclesRemaining = 2.5f / electro.TypeInfo.TimedLife;
      }
    }
  }
}

