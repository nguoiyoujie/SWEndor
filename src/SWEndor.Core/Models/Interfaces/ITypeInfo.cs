namespace SWEndor.Models
{
  /// <summary>
  /// Represents an in-game object type
  /// </summary>
  public interface ITypeInfo<T>
  {
    ComponentMask Mask { get; }
  }
}