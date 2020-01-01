using SWEndor.Core;

namespace SWEndor.Models
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