using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public interface IHealth : ITrait
  {
    void Init(ActorTypeInfo type, ActorCreationInfo acinfo);

    float HP { get; }
    float MaxHP { get; }
    float DisplayHP { get; }
    float Level { get; }
    bool IsDead { get; }

    float Frac { get; }
    float Perc { get; }

    float DisplayFrac { get; }
    float DisplayPerc { get; }

    TV_COLOR Color { get; }

    void InflictDamage<A1, A2>(A1 self, DamageInfo<A2> dmg) where A1 : class, ITraitOwner where A2 : class, ITraitOwner;
    void Kill<A1, A2>(A1 self, A2 attacker) where A1 : class, ITraitOwner where A2 : class, ITraitOwner;
    void SetHP<A1>(A1 self, float value) where A1 : class, ITraitOwner;
    void SetMaxHP<A1>(A1 self, float value, bool scaleHP) where A1 : class, ITraitOwner;
  }
}
