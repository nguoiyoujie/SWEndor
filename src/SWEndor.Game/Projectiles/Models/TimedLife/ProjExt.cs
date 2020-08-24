namespace SWEndor.Game.Projectiles
{
  public partial class ProjectileInfo
  {
    public float DyingDuration { get { return DyingTimer.Interval; } }
    public float DyingTimeRemaining { get { return DyingTimer.TimeRemaining; } }
    public void DyingTimerStart() { DyingTimer.Start(); }
    public void DyingTimerPause() { DyingTimer.Pause(); }
    public void DyingTimerSet(float time, bool startnow) { DyingTimer.Set(time, startnow); }
  }
}
