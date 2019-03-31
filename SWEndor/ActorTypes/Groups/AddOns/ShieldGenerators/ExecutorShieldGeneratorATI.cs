﻿using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class ExecutorShieldGeneratorATI : AddOnGroup
  {
    private static ExecutorShieldGeneratorATI _instance;
    public static ExecutorShieldGeneratorATI Instance()
    {
      if (_instance == null) { _instance = new ExecutorShieldGeneratorATI(); }
      return _instance;
    }

    private ExecutorShieldGeneratorATI() : base("Executor Super Star Destroyer Shield Generator")
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

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (!hitby.TypeInfo.IsDamage)
      {
        ainfo.CombatInfo.Strength = 0;
      }
      base.ProcessHit(ainfo, hitby, impact, normal);
      if (hitby.TypeInfo.IsDamage)
      {
        foreach (ActorInfo child in ainfo.GetAllChildren(1))
        {
          if (child.TypeInfo is ElectroATI)
          {
            child.CycleInfo.CyclesRemaining = 2.5f / child.TypeInfo.TimedLife;
            return;
          }
        }
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("Electro"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo electro = ActorInfo.Create(acinfo);
        electro.AddParent(ainfo);
        electro.CycleInfo.CyclesRemaining = 2.5f / electro.TypeInfo.TimedLife;
      }
    }
  }
}

