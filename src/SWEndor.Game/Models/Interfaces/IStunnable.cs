namespace SWEndor.Game.Models
{
  /// <summary>
  /// Represents a object supporting a dying timer
  /// </summary>
  public interface IStunnable
  {
    bool IsStunned { get; }
    void InflictStun(float stunduration);
  }
}