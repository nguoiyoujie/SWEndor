using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Groups
{
  public class MissileProjectile : Projectile
  {
    internal MissileProjectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      Armor = ArmorInfo.Default;
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      CullDistance = 12000;

      // Projectile
      ImpactCloseEnoughDistance = 100;
      TargetType = TargetType.MUNITION;
      RadarType = RadarType.TRAILLINE;

      Mask = ComponentMask.GUIDED_PROJECTILE;

      TrackerDummyWeapon = true;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CurrentAction == null || ainfo.CurrentAction is Idle)
        ainfo.SetState_Dead();
    }
  }
}


