using SWEndor.Primitives;
using SWEndor.UI.Menu.Pages;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace SWEndor
{
  public class Game
  {
    public readonly Engine Engine;
    internal Game(Engine engine)
    {
      TimeSinceRender = 1;
      Engine = engine;

      // setup threads
      th_load = new Thread(new ThreadStart(Engine.Load));

      // setup process timers
      tm_sound = new System.Timers.Timer(30);
      tm_ai = new System.Timers.Timer(30);
      tm_collision = new System.Timers.Timer(30);
      tm_render = new System.Timers.Timer(30);
      tm_process = new System.Timers.Timer(30);
      tm_perf = new System.Timers.Timer(1000);

      tm_sound.Elapsed += TimerSound;
      tm_ai.Elapsed += TimerAI;
      tm_collision.Elapsed += TimerCollision;
      tm_render.Elapsed += TimerRender;
      tm_process.Elapsed += TimerProcess;
      tm_perf.Elapsed += TimerPerf;

      // time control
      TimeControl = new TimeControl();
    }

    public TimeControl TimeControl;

    public float GameTime { get; private set; }

    private enum RunState { STOPPED, LOADING, RUNNING }
    private RunState State { get; set; } = RunState.STOPPED;

    public float TimeSinceRender { get; set; }
    public float AddTime { get; set; }
    public bool IsPaused { get; set; }

    private Thread th_load { get; set; }
    private Thread th_tick { get; set; }

    private readonly System.Timers.Timer tm_sound;
    private readonly System.Timers.Timer tm_ai;
    private readonly System.Timers.Timer tm_collision;
    private readonly System.Timers.Timer tm_render;
    private readonly System.Timers.Timer tm_process;
    private readonly System.Timers.Timer tm_perf;

    public float CurrentFPS { get { return TimeControl.FPS; } } //{ get; private set; }

    private bool septhread_sound = true;
    private bool septhread_ai = true;
    private bool septhread_collision = false; // don't set to true, weird things happen if collision is out of sync
    private bool septhread_render = false;
    private bool septhread_process = false;

    private bool isProcessingPerf = false;
    private bool isProcessingAI = false;
    private bool isProcessingCollision = false;
    private bool isProcessingRender = false;
    private bool isProcessingProcess = false;

    public bool EnableSound = true;
    public bool EnablePerf = true;
    public bool EnableAI = true;
    public bool EnableCollision = true;

    /// <summary>
    /// Checks if the current FPS is below the LowFPSLimit
    /// </summary>
    /// <returns></returns>
    public bool IsLowFPS()
    {
      return CurrentFPS < TimeControl.MinimumFPS;
    }

    /// <summary>
    /// Determines the modifiers to render culling distance to increase performance
    /// </summary>
    public float PerfCullModifier
    {
      get
      {
        return (CurrentFPS < TimeControl.PerformanceSavingFPS)
          ? (CurrentFPS / TimeControl.PerformanceSavingFPS < 0.5f) ? 0.5f
          : CurrentFPS / TimeControl.PerformanceSavingFPS : 1;
      }
    }

    public void StartLoad()
    {
      State = RunState.LOADING;
    }

    public void Run()
    {
      State = RunState.RUNNING;
      tm_perf.Start();
      Task.Factory.StartNew(new Action(Tick)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);
    }

    public void Stop()
    {
      State = RunState.STOPPED;
    }

    public void PrepExit()
    {
      tm_ai.Stop();
      tm_collision.Stop();
      tm_render.Stop();
      tm_process.Stop();
      tm_perf.Stop();
      tm_sound.Stop();
      Engine.SoundManager.Update();
      Thread.Sleep(1500);
    }

    private void Tick()
    {
      Thread.CurrentThread.Name = "Main Loop";
      Thread.Sleep(1000);

      Engine.PerfManager.ClearPerf();

      // Pre-load
      th_load.Start();
      while (th_load.ThreadState == ThreadState.Running)
      {
        Engine.PreRender();
        TimeControl.Update();
        TimeControl.Wait();

        Engine.InputManager.ClearInput();
      }

      // Initialize other threads/timers
      if (septhread_sound)
        tm_sound.Start();

      if (septhread_ai)
        tm_ai.Start();

      if (septhread_collision)
        tm_collision.Start();

      if (septhread_render)
        tm_render.Start();

      if (septhread_process)
        tm_process.Start();

      try
      {
        while (State == RunState.RUNNING)
        {
          try
          {
            using (Engine.PerfManager.Create("tick_loop"))
            {
              TimeControl.Update();
              //TimeSinceRender = (1f / TimeControl.TargetFPS) * TimeControl.SpeedModifier; 
              TimeSinceRender = TimeControl.WorldInterval;
              TimeControl.AddTime(AddTime);
              TimeSinceRender += AddTime;
              AddTime = 0;

              if (!septhread_collision)
                TickCollision();

              if (!septhread_process)
                TickProcess();

              if (!septhread_ai)
                TickAI();

              if (!septhread_sound)
                TickSound();

              if (!septhread_render)
                TickRender();

              if (!IsPaused)
                GameTime += TimeSinceRender;
            }
          }
          catch (Exception ex)
          {
            GenerateFatalError(ex);
          }
        }
      }
      catch (Exception ex)
      {
        Log.WriteErr(Log.ERROR, ex);
        MessageBox.Show("Fatal Error occurred during runtime. Please see {0} in the /Log folder for the error message.".F(Log.ERROR)
                      , Application.ProductName + " - Error Encountered!"
                      , MessageBoxButtons.OK);
        return;
      }
      finally
      {
        Engine.Exit();
      }
    }

    private void GenerateFatalError(Exception ex)
    {
      // Replace this block to print this on file!
      if (State == RunState.RUNNING)
      {
        Engine.Screen2D.CurrentPage = new FatalError(Engine.Screen2D, ex);
        Engine.Screen2D.ShowPage = true;
        IsPaused = true;
      }
    }

    private void TimerSound(object sender, ElapsedEventArgs e)
    {
      TickSound();
      if (State != RunState.RUNNING)
        tm_sound.Stop();
    }

    private void TimerAI(object sender, ElapsedEventArgs e)
    {
      TickAI();
      if (State != RunState.RUNNING)
        tm_ai.Stop();
    }

    private void TimerCollision(object sender, ElapsedEventArgs e)
    {
      TickCollision();
      if (State != RunState.RUNNING)
        tm_collision.Stop();
    }

    private void TimerRender(object sender, ElapsedEventArgs e)
    {
      TickRender();
      if (State != RunState.RUNNING)
        tm_render.Stop();
    }

    private void TimerProcess(object sender, ElapsedEventArgs e)
    {
      TickProcess();
      if (State != RunState.RUNNING)
        tm_process.Stop();
    }

    private void TimerPerf(object sender, ElapsedEventArgs e)
    {
      TickPerf();
      if (State != RunState.RUNNING)
        tm_perf.Stop();
    }

    #region Tick Functions

    private void TickRender()
    {
      try
      {
        if (!isProcessingRender)
        {
          isProcessingRender = true;
          using (Engine.PerfManager.Create("render"))
          {
            using (Engine.PerfManager.Create("render_main"))
              Engine.Render();

            if (!IsPaused)
              using (Engine.PerfManager.Create("render_camera"))
                Engine.PlayerCameraInfo.Update();
          }
          isProcessingRender = false;
        }
      }
      catch (Exception ex)
      {
        GenerateFatalError(ex);
      }
    }

    private void TickProcess()
    {
      try
      {
        if (!isProcessingProcess)
        {
          isProcessingProcess = true;

          using (Engine.PerfManager.Create("tick_process"))
          {
            using (Engine.PerfManager.Create("process_input"))
              Engine.InputManager.ProcessInput();

            if (!IsPaused)
            {
              using (Engine.PerfManager.Create("process_main"))
                Engine.Process();

                using (Engine.PerfManager.Create("process_player"))
                  Engine.PlayerInfo.Update();
            }

            using (Engine.PerfManager.Create("process_page"))
              Engine.Screen2D.CurrentPage?.Tick();

            if (!IsPaused)
            {
              using (Engine.PerfManager.Create("process_returnactorpool"))
                Engine.ActorFactory.ReturnToPool();

              using (Engine.PerfManager.Create("process_activateactor"))
                Engine.ActorFactory.ActivatePlanned();

              using (Engine.PerfManager.Create("process_destroyactor"))
                Engine.ActorFactory.DestroyDead();

              using (Engine.PerfManager.Create("process_scenario"))
                Engine.GameScenarioManager.Update();
            }
          }
          isProcessingProcess = false;
        }
      }
      catch (Exception ex)
      {
        GenerateFatalError(ex);
        isProcessingProcess = false;
      }
    }

    private void TickAI()
    {
      try
      {
        if (EnableAI)
          if (!isProcessingAI && !IsPaused)
          {
            isProcessingAI = true;
            using (Engine.PerfManager.Create("tick_ai"))
              Engine.ProcessAI();
            isProcessingAI = false;
          }
      }
      catch (Exception ex)
      {
        GenerateFatalError(ex);
        EnableAI = false;
      }
    }

    private void TickSound()
    {
      try
      {
        if (EnableSound)
          if (Engine.SoundManager.PendingUpdate)
            using (Engine.PerfManager.Create("tick_sound"))
              Engine.SoundManager.Update();
      }
      catch (Exception ex)
      {
        GenerateFatalError(ex);
        EnableSound = false;
      }
    }

    private void TickCollision()
    {
      try
      {
        if (EnableCollision)
          if (!isProcessingCollision && !IsPaused)
          {
            isProcessingCollision = true;
            using (Engine.PerfManager.Create("tick_collision"))
              Engine.ProcessCollision();
            isProcessingCollision = false;
          }
      }
      catch (Exception ex)
      {
        GenerateFatalError(ex);
        EnableCollision = false;
      }
    }

    private void TickPerf()
    {
      if (EnablePerf)
        if (!isProcessingPerf)
        {
          isProcessingPerf = true;
          using (Engine.PerfManager.Create("tick_perf"))
            Engine.PerfManager.PrintPerf();
          Thread.Sleep(1000);
          isProcessingPerf = false;
        }
    }
    #endregion
  }
}


