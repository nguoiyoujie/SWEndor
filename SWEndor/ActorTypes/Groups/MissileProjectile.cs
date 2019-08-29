using SWEndor.Actors;
using SWEndor.Actors.Components;
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
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.TIMENOTEXPIRED_ONLY, deathExplosionType: "ExplosionSm");
      Armor = ArmorInfo.Default;
      CombatData = CombatData.DefaultFighter;

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


