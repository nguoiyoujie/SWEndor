namespace SWEndor.Actors.Types
{
  public class SurfaceTowerGroup : ActorTypeInfo
  {
    internal SurfaceTowerGroup(string name) : base(name)
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

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
    }
  }
}

