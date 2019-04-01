using SWEndor.Actors;

namespace SWEndor.ActorTypes
{
  public class DebrisGroup : ActorTypeInfo
  {
    internal DebrisGroup(string name) : base(name)
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

