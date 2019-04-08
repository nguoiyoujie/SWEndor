using SWEndor.Actors;

namespace SWEndor.ActorTypes
{
  public class AsteroidGroup : ActorTypeInfo
  {
    internal AsteroidGroup(string name) : base(name)
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
      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
    }
  }
}

