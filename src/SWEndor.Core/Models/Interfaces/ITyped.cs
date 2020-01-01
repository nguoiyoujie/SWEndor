namespace SWEndor.Models
{
  /// <summary>
  /// Represents an in-game typed object
  /// </summary>
  public interface ITyped<TType>
  {
    /// <summary>The instance type of this instance</summary>
    TType TypeInfo { get; }
  }
}