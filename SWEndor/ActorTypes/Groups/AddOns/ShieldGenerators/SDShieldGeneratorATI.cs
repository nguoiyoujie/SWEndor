﻿using MTV3D65;
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
      ainfo.RegenerationInfo.ParentRegenRate = 3.5f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.3f;

      ainfo.Scale *= 0.9f;
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = ActorInfo.Factory.Get(ownerActorID);
      ActorInfo hitby = ActorInfo.Factory.Get(hitbyActorID);

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
          ActorInfo child = ActorInfo.Factory.Get(i);
          if (child?.TypeInfo is ElectroATI)
          {
            child.CycleInfo.CyclesRemaining = 2.5f / child.CycleInfo.CyclePeriod;
            return;
          }
        }
        ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
        acinfo.Position = owner.GetPosition();
        ActorInfo electro = ActorInfo.Create(acinfo);
        electro.AddParent(ownerActorID);
        electro.CycleInfo.CyclesRemaining = 2.5f / electro.CycleInfo.CyclePeriod;
      }
    }
  }
}

