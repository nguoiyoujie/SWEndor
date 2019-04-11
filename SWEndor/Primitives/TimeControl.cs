using System.Diagnostics;
using System.Threading;

namespace SWEndor.Primitives
{
  public class TimeControl
  {
    //private Engine _engine;
    private uint _targetFPS;
    public uint MinimumFPS { get; set; } = 15;
    public uint MaximumFPS { get; set; } = 90;
    public uint PerformanceSavingFPS { get; set; } = 25;
    public float FPS { get; private set; }
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

    public TimeControl() { TargetFPS = 60; }

    private object waitlock = new object();
    private Stopwatch stopwatch = Stopwatch.StartNew();

    public float SpeedModifier = 1;

    public float WorldTime { get; private set; } = 0;

    public float RenderInterval { get; private set; }
    public float WorldInterval { get { return RenderInterval * SpeedModifier; } }

    private int _FPScounter;
    private float _FPScountTime;
    private float _FPSrefreshInterval = 0.2f;

    public void Update()
    {
      RenderInterval = (float)stopwatch.Elapsed.TotalSeconds;
      if (RenderInterval < 1f / MaximumFPS)
        RenderInterval = 1f / MaximumFPS;

      if (RenderInterval > 1f / MinimumFPS)
        RenderInterval = 1f / MinimumFPS;

      stopwatch.Restart();
      if (_FPScountTime > _FPSrefreshInterval)
      {
        FPS = _FPScounter / _FPScountTime;

        _FPScounter = 0;
        _FPScountTime = 0;
      }

      _FPScounter++;
      _FPScountTime += RenderInterval;
      WorldTime += WorldInterval;
    }

    public void AddTime(float worldtime)
    {
      WorldTime += worldtime;
    }

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
