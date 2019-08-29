using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Instances
{
  public class DSLaserSourceATI : ActorTypeInfo //: AddOnGroup
  {
    WeaponShotInfo laser;

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

      laser = new WeaponShotInfo(WeaponFactory.Get("DSTAR_LSR"), 1);

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
          FireWeapon(ainfo, target, laser);
        }
      }
    }
  }
}

