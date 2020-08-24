namespace SWEndor.Game.Models
{
  /// <summary>
  /// Represents an in-game linked object. Used by the owning factory classes
  /// </summary>
  public interface ILinked<T>
  {
    /// <summary>The previous linked instance</summary>
    T Prev { get; set; }

    /// <summary>The next linked instance</summary>
    T Next { get; set; }
  }
}