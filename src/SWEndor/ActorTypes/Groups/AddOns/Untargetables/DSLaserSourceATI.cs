using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Weapons;

namespace SWEndor.ActorTypes.Instances
{
  public class DSLaserSourceATI : ActorTypeInfo //: AddOnGroup
  {
    WeaponShotInfo laser;

    internal DSLaserSourceATI(Factory owner) : base(owner, "DSLSRSRC", "Death Star Laser Source")
    {
      SystemData.MaxShield = 1500.0f;
      CombatData.ImpactDamage = 4000.0f;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      laser = new WeaponShotInfo(ainfo.Engine.WeaponFactory.Get("DSTAR_LSR"), 1);

      ainfo.QueueNext(Lock.GetOrCreate());
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      
      if (ainfo.CurrentAction != null && ainfo.CurrentAction is AttackActor)
      {
        ActorInfo target = engine.ActorFactory.Get(((AttackActor)ainfo.CurrentAction).Target_ActorID);
        if (target != null && target.Active)
        {
          FireWeapon(engine, ainfo, target, laser);
        }
      }
    }
  }
}

