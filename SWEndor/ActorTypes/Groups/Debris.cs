using SWEndor.Actors;

namespace SWEndor.ActorTypes.Groups
{
  public class Debris : ActorTypeInfo
  {
    internal Debris(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      CullDistance = 4500f;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CombatInfo.DamageModifier = 0;
    }
  }
}

