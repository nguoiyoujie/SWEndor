namespace SWEndor.Actors.Types
{
  public class AddOnGroup : ActorTypeInfo
  {
    internal AddOnGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CullDistance = 3500;

      RadarSize = 1;

      Attack_AngularDelta = 360f;
      Attack_HighAccuracyAngularDelta = 360f;

      TargetType = TargetType.ADDON;

      NoMove = true;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
    }
  }
}

