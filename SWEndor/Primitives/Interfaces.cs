namespace SWEndor.Primitives
{
  /// <summary>
  /// Implements an ID object
  /// </summary>
  public interface IIdentity { int ID { get; } }

  /// <summary>
  /// Implements a named object
  /// </summary>
  public interface INamedObject { string Name { get; } }
}
