using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class SpinningDebris : Debris
  {
    internal SpinningDebris(Factory owner, string name) : base(owner, name)
    {
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      OnTimedLife = true;
      TimedLife = 5f;

      MaxSpeed = 500;
      MinSpeed = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.MovementInfo.DyingMovement = DyingMovement.SPIN;
      ainfo.MovementInfo.D_spin_min_rate = 120;
      ainfo.MovementInfo.D_spin_max_rate = 270;

      ainfo.ExplosionInfo.Active = true;
      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
    }
  }
}

