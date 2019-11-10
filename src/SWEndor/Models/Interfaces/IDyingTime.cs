namespace SWEndor.Models
{
  /// <summary>
  /// Represents a object supporting a dying timer
  /// </summary>
  public interface IDyingTime
  {
    float DyingDuration { get; }
    float DyingTimeRemaining { get; }
  }
}