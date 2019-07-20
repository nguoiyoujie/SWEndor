using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public interface IStateModel : ITrait
  {
    void Init(ActorTypeInfo type, ActorCreationInfo acinfo);

    ActorState ActorState { get; }
    CreationState CreationState { get; set; }
    ComponentMask ComponentMask { get; set; }
    float CreationTime { get; }

    void AdvanceDeathOneLevel<A>(A self) where A : ITraitOwner;
    void MakeDead<A>(A self) where A : ITraitOwner;
    void MakeDying<A>(A self) where A : ITraitOwner;
    void MakeNormal<A>(A self) where A : ITraitOwner;
    //void MakeFree<A>(A self) where A : ITraitOwner;

    bool IsDead { get; }
    bool IsDying { get; }
    bool IsDyingOrDead { get; }
  }
}
