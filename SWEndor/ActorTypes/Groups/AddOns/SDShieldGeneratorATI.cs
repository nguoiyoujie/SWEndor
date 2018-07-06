using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
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

      CullDistance = 20000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.AllowRegen = false;
      ainfo.ParentRegenRate = 35f;
      ainfo.RelativeRegenRate = 0.1f;
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (!hitby.TypeInfo.IsDamage)
      {
        ainfo.Strength = 0;
      }
      base.ProcessHit(ainfo, hitby, impact, normal);
      if (hitby.TypeInfo.IsDamage)
      {
        foreach (ActorInfo child in ainfo.GetAllChildren(1))
        {
          if (child.TypeInfo is ElectroATI)
          {
            child.SetStateF("CyclesRemaining", 2.5f / child.TypeInfo.TimedLife);
            return;
          }
        }
        ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo electro = ActorInfo.Create(acinfo);
        electro.AddParent(ainfo);
        electro.SetStateF("CyclesRemaining", 2.5f / electro.TypeInfo.TimedLife);
      }
    }
  }
}

