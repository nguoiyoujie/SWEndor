﻿using SWEndor.ExplosionTypes;

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo
  {
    public float DyingTimeRemaining { get { return DyingTimer.TimeRemaining; } }
    public void DyingTimerStart() { DyingTimer.Start(); }
    public void DyingTimerPause() { DyingTimer.Pause(); }
    public void DyingTimerSet(float time, bool startnow) { DyingTimer.Set(time, startnow); }
  }
}
