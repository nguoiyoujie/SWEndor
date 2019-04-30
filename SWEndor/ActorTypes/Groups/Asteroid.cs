using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class Asteroid : ActorTypeInfo
  {
    internal Asteroid(Factory owner, string name) : base(owner, name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = true;
      CullDistance = 4500;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CombatInfo.DamageModifier = 0;
      ainfo.ExplosionInfo.DeathExplosionTrigger = DeathExplosionTrigger.ALWAYS;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
    }
  }
}

