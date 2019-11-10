namespace Primrose.Primitives
{
  /// <summary>
  /// Implements an ID object
  /// </summary>
  public interface IIdentity
  {
    /// <summary>The instance ID</summary>
    short ID { get; }
  }

  /// <summary>
  /// Implements a named object
  /// </summary>
  public interface INamedObject
  {
    /// <summary>The instance name</summary>
    string Name { get; }
  }
}
