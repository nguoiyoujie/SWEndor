using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDLowerShieldGeneratorATI : Groups.AddOn
  {
    internal SDLowerShieldGeneratorATI(Factory owner) : base(owner, "Star Destroyer Lower Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 90;
      ImpactDamage = 300.0f;
      RadarSize = 2.5f;

      CullDistance = 8000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      TargetType |= TargetType.SHIELDGENERATOR;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_lower_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.RegenerationInfo.AllowRegen = false;
      ainfo.RegenerationInfo.ParentRegenRate = 20f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.5f;
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = this.GetEngine().ActorFactory.Get(ownerActorID);
      ActorInfo hitby = this.GetEngine().ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (!hitby.TypeInfo.IsDamage)
        owner.CombatInfo.Strength = 0;

      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);

      if (hitby.TypeInfo.IsDamage)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("Electro"));
        acinfo.Position = owner.GetPosition();
        ActorInfo electro = ActorInfo.Create(this.GetEngine().ActorFactory, acinfo);
        electro.AddParent(owner.ID);
        electro.CycleInfo.CyclesRemaining = 2.5f / electro.TypeInfo.TimedLife;
      }
    }
  }
}

