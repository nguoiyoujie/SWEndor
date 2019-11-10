using System.Diagnostics;
using System.Threading;

namespace Primrose.Primitives
{
  /// <summary>
  /// Represents a time control 
  /// </summary>
  public class TimeControl
  {
    private uint _targetFPS;
    private int _FPScounter;
    private float _FPScountTime;
    private float _FPSrefreshInterval = 0.2f;
    private object waitlock = new object();
    private Stopwatch stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Initializes a time control
    /// </summary>
    public TimeControl() { TargetFPS = 60; }

    /// <summary>Defines the minimum desirable FPS</summary>
    public uint MinimumFPS { get; set; } = 15;

    /// <summary>Defines the maximum desirable FPS</summary>
    public uint MaximumFPS { get; set; } = 90;

    /// <summary>Defines the FPS where performance savings should be triggered</summary>
    public uint PerformanceSavingFPS { get; set; } = 25;

    /// <summary>The current FPS</summary>
    public float FPS { get; private set; }

    /// <summary>The target FPS</summary>
    public uint TargetFPS
    {
      get
      {
        return _targetFPS;
      }
      set
      {
        if (value < MinimumFPS)
          value = MinimumFPS;
        _targetFPS = value;
      }
    }

    /// <summary>A multiplier to world time</summary>
    public float SpeedModifier = 1;

    /// <summary>The world time</summary>
    public float WorldTime { get; private set; } = 0;

    /// <summary>A interval between two successive updates</summary>
    public float UpdateInterval { get; private set; }
    
    /// <summary>The interval in world time</summary>
    public float WorldInterval { get { return UpdateInterval * SpeedModifier; } }

    /// <summary>Updates the time</summary>
    public void Update()
    {
      UpdateInterval = (float)stopwatch.Elapsed.TotalSeconds;
      if (UpdateInterval < 1f / MaximumFPS)
        UpdateInterval = 1f / MaximumFPS;

      if (UpdateInterval > 1f / MinimumFPS)
        UpdateInterval = 1f / MinimumFPS;

      stopwatch.Restart();
      if (_FPScountTime > _FPSrefreshInterval)
      {
        FPS = _FPScounter / _FPScountTime;

        _FPScounter = 0;
        _FPScountTime = 0;
      }

      _FPScounter++;
      _FPScountTime += UpdateInterval;
      WorldTime += WorldInterval;
    }

    /// <summary>Performs a time skip to increment the time</summary>
    /// <param name="worldtime"></param>
    public void AddTime(float worldtime)
    {
      WorldTime += worldtime;
    }

    /// <summary>Performs a wait to suspend process until the target FPS is reached</summary>
    public void Wait()
    {
      long target = 1000 / TargetFPS;
      long passed = stopwatch.ElapsedMilliseconds;
      int wait = (int)(target - passed);
      if (wait > 1)
        Thread.Sleep(wait);
    }
  }
}
