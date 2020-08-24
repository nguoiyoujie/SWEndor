using SWEndor.Game.Core;

namespace SWEndor.Game.Models
{
  /// <summary>
  /// Represents a Engine supported object
  /// </summary>
  public interface IEngineObject
  {
    /// <summary>The game engine</summary>
    Engine Engine { get; }
  }
}