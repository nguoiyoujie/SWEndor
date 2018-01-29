﻿using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class SDLowerShieldGeneratorATI : AddOnGroup
  {
    private static SDLowerShieldGeneratorATI _instance;
    public static SDLowerShieldGeneratorATI Instance()
    {
      if (_instance == null) { _instance = new SDLowerShieldGeneratorATI(); }
      return _instance;
    }

    private SDLowerShieldGeneratorATI() : base("Star Destroyer Lower Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 120.0f;
      ImpactDamage = 300.0f;
      RadarSize = 2.5f;

      CullDistance = 8000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_lower_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.AllowRegen = false;
      ainfo.ParentRegenRate = 35f;
      ainfo.RelativeRegenRate = 0.5f;
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ainfo, hitby, impact, normal);
      if (!hitby.TypeInfo.IsDamage)
      {
        ainfo.ActorState = ActorState.DEAD;
      }
      else
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("Electro"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo electro = ActorInfo.Create(acinfo);
        electro.AddParent(ainfo);
        electro.SetStateF("CyclesRemaining", 2.5f / electro.TypeInfo.TimedLife);
      }
    }
  }
}

