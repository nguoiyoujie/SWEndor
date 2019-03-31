using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Actors.Types
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

      TargetType = TargetType.NULL;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new DeathStarLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:laser" };

      ActionManager.QueueNext(ainfo, new Lock());
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      
      if (ainfo.CurrentAction != null && ainfo.CurrentAction is AttackActor)
      {
        AttackActor a = (AttackActor)ainfo.CurrentAction;
        if (a.Target_Actor.CreationState == CreationState.ACTIVE)
        {
          FireWeapon(ainfo, a.Target_Actor, "auto");
        }
      }
    }
  }
}

