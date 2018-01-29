using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class DSLaserSourceATI : ActorTypeInfo //: AddOnGroup
  {
    private static DSLaserSourceATI _instance;
    public static DSLaserSourceATI Instance()
    {
      if (_instance == null) { _instance = new DSLaserSourceATI(); }
      return _instance;
    }

    private DSLaserSourceATI() : base("Death Star Laser Source")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 1500.0f;
      ImpactDamage = 4000.0f;

      EnableDistanceCull = false;
      CullDistance = 40000;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new DeathStarLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };

      ActionManager.QueueNext(ainfo, new Actions.Lock());
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      
      if (ainfo.CurrentAction != null && ainfo.CurrentAction is Actions.AttackActor)
      {
        Actions.AttackActor a = (Actions.AttackActor)ainfo.CurrentAction;
        if (a.Target_Actor.CreationState == CreationState.ACTIVE)
        {
          FireWeapon(ainfo, a.Target_Actor, "auto");
        }
      }
    }
  }
}

