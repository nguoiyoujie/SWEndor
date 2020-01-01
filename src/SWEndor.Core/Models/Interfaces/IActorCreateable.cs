using SWEndor.Core;

namespace SWEndor.Models
{
  /// <summary>
  /// Represents a created instance object
  /// </summary>
  /// <typeparam name="TCreate">The creator data type</typeparam>
  public interface IActorCreateable<TCreate>
  {
    /// <summary>The in-game time when the object was created or rebuilt</summary>
    float CreationTime { get; }

    /// <summary>
    /// Rebuilds the instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="id">The new ID</param>
    /// <param name="acinfo">The instance creation data</param>
    void Rebuild(Engine engine, short id, TCreate acinfo);

    /// <summary>
    /// Initializes the game object instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    void Initialize(Engine engine);
  }
}