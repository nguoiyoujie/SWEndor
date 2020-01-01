namespace SWEndor.Models
{
  /// <summary>
  /// Represents an object supporting a parent object
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IParent<T>
  {
    /// <summary>The parent of this object when considering coordinate positions</summary>
    T ParentForCoords { get; }
  }
}