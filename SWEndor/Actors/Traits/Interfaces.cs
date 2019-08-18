using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Primitives.Traits;
using System;

namespace SWEndor.Actors.Traits
{
  public interface ITick : ITrait { void Tick<A>(A self, float time) where A : class, ITraitOwner; }

  public interface IDisposableTrait : ITrait, IDisposable { }

  // CreationState
  public interface INotifyPlanned : ITrait { void Planned<A1>(A1 self) where A1 : class, ITraitOwner; }
  public interface INotifyCreated : ITrait { void Created<A1>(A1 self) where A1 : class, ITraitOwner; }
  public interface INotifyDisposed : ITrait { void Disposed<A1>(A1 self) where A1 : class, ITraitOwner; }

  public interface INotifyExitHyperspace : ITrait { void ExitHyperspace<A1>(A1 self) where A1 : class, ITraitOwner; }
  public interface INotifyEnterHyperspace : ITrait { void EnterHyperspace<A1>(A1 self) where A1 : class, ITraitOwner; }

  // ActorState
  public interface INotifyActorStateChanged : ITrait { void ActorStateChanged<A1>(A1 self) where A1 : class, ITraitOwner; }


  public interface INotifyDying : ITrait { void Dying<A1>(A1 self) where A1 : class, ITraitOwner; }
  public interface INotifyDead : ITrait { void Dead<A1>(A1 self) where A1 : class, ITraitOwner; }

  // Damage
  public interface INotifyDamage : ITrait { void Damaged<A1, A2>(A1 self, DamageInfo<A2> e) where A1 : class, ITraitOwner where A2 : class, ITraitOwner; }
  public interface INotifyKilled : ITrait { void Killed<A1, A2>(A1 self, DamageInfo<A2> e) where A1 : class, ITraitOwner where A2 : class, ITraitOwner; }
  public interface INotifyAppliedDamage : ITrait { void AppliedDamage<A1, A2>(A2 attacker, A1 target, DamageInfo<A2> e) where A1 : class, ITraitOwner where A2 : class, ITraitOwner; }

  public interface INotifyFactionChanged : ITrait { void OnOwnerChanged<A1>(A1 self, FactionInfo oldOwner, FactionInfo newOwner) where A1 : class, ITraitOwner; }








}
