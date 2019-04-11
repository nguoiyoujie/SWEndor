using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class SDShieldGeneratorATI : AddOnGroup
  {
    private static SDShieldGeneratorATI _instance;
    public static SDShieldGeneratorATI Instance()
    {
      if (_instance == null) { _instance = new SDShieldGeneratorATI(); }
      return _instance;
    }

    private SDShieldGeneratorATI() : base("Star Destroyer Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 105;
      ImpactDamage = 300.0f;
      RadarSize = 2;

      CullDistance = 30000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      TargetType |= TargetType.SHIELDGENERATOR;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.RegenerationInfo.AllowRegen = false;
      ainfo.RegenerationInfo.ParentRegenRate = 15f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.5f;
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (!hitby.TypeInfo.IsDamage)
      {
        ainfo.CombatInfo.Strength = 0;
      }
      base.ProcessHit(ainfo, hitby, impact, normal);
      if (hitby.TypeInfo.IsDamage)
      {
        foreach (int i in ainfo.GetAllChildren(1))
        {
          ActorInfo child = ActorInfo.Factory.GetExact(i);
          if (child?.TypeInfo is ElectroATI)
          {
            child.CycleInfo.CyclesRemaining = 2.5f / child.CycleInfo.CyclePeriod;
            return;
          }
        }
        ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo electro = ActorInfo.Create(acinfo);
        electro.AddParent(ainfo);
        electro.CycleInfo.CyclesRemaining = 2.5f / electro.CycleInfo.CyclePeriod;
      }
    }
  }
}

