namespace SWEndor.Models
{
  /// <summary>
  /// Represents an in-game creation data type
  /// </summary>
  public interface ICreationInfo<T, TType> where TType : ITypeInfo<T>
  {
    /// <summary>The instance type of this instance</summary>
    TType TypeInfo { get; }
  }
}