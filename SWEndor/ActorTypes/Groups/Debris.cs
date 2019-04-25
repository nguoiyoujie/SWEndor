using SWEndor.Actors;

namespace SWEndor.ActorTypes.Groups
{
  public class Debris : ActorTypeInfo
  {
    internal Debris(Factory owner, string name) : base(owner, name)
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

