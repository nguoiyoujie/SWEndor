using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class SurfaceTower : ActorTypeInfo
  {
    internal SurfaceTower(Factory owner, string name) : base(owner, name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;
      CullDistance = 10000;

      RadarSize = 1.5f;

      TargetType = TargetType.ADDON | TargetType.STRUCTURE;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.ExplosionInfo.DeathExplosionTrigger = DeathExplosionTrigger.ALWAYS;
    }
  }
}

