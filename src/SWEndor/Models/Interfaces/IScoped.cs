using Primrose.Primitives;

namespace SWEndor.Models
{
  /// <summary>
  /// Represents an in-game scoped object
  /// </summary>
  public interface IScoped
  {
    /// <summary>A scope counter determining whether the object is still in scope or safe to dispose</summary>
    ScopeCounters.ScopeCounter Scope { get; }
  }
}