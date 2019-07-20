using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.Traits
{
  /// <summary>
  /// Implements an ITrait owner
  /// </summary>
  public interface ITraitOwner : IDisposable, IIdentity, INamedObject
  {
    Engine Engine { get; }
    bool Disposed { get; }
    ITraitOwner Owner { get; }

    T Trait<T>() where T : ITrait;
    bool TryGetTrait<T>(out T trait) where T : ITrait;
    T TraitOrDefault<T>() where T : ITrait;
    IEnumerable<T> TraitsImplementing<T>() where T : ITrait;
    T AddTrait<T>(T trait) where T : ITrait;
  }

  /// <summary>
  /// Identifies the concrete object as a trait, allowing attachment to an ITraitOwner
  /// </summary>
  public interface ITrait { }
}
