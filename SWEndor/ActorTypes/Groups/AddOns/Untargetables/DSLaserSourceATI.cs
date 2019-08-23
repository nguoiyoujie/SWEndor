using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Instances
{
  public class DSLaserSourceATI : ActorTypeInfo //: AddOnGroup
  {
    internal DSLaserSourceATI(Factory owner) : base(owner, "Death Star Laser Source")
    {
      MaxStrength = 1500.0f;
      ImpactDamage = 4000.0f;

      EnableDistanceCull = false;

      TargetType = TargetType.NULL;
      RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("DSTAR_LSR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "none" };

      ainfo.QueueNext(new Lock());
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      
      if (ainfo.CurrentAction != null && ainfo.CurrentAction is AttackActor)
      {
        ActorInfo target = ActorFactory.Get(((AttackActor)ainfo.CurrentAction).Target_ActorID);
        if (target != null && target.Active)
        {
          FireWeapon(ainfo, target, "laser");
        }
      }
    }
  }
}

