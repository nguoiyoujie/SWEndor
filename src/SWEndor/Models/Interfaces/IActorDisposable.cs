namespace SWEndor.Models
{
  /// <summary>
  /// Represents an in-game disposable object
  /// </summary>
  public interface IActorDisposable
  {
    /// <summary>Returns whether the creation state is DISPOSING or already DISPOSED</summary>
    bool DisposingOrDisposed { get; }

    /// <summary>Returns whether the creation state is DISPOSED</summary>
    bool Disposed { get; }

    /// <summary>Disposes the object</summary>
    void Destroy();

    /// <summary>Mark the object for disposal</summary>
    void Delete();
  }
}