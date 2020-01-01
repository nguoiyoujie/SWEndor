namespace SWEndor.Models
{
  /// <summary>
  /// Represents an object that notifies state changes
  /// </summary>
  public interface INotify
  {
    void OnStateChangeEvent();
  }
}