using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace SWEndor
{
  public class Game
  {
    private static Game _instance;
    public static Game Instance()
    {
      if (_instance == null) { _instance = new Game(); }
      return _instance;
    }

    private Game()
    {
      CurrentFPS = 30;
      TimeSinceRender = 1;
      FPSLimit = 60; //40
      RenderLowFPSLimit = 60; //40
      LowFPSLimit = 36;

      th_load = new Thread(new ThreadStart(Engine.Instance().Load));
      th_tick = new Thread(new ThreadStart(Tick));

      tm_sound = new System.Timers.Timer(50);
      tm_sound.Elapsed += TimerSound;

      tm_ai = new System.Timers.Timer(50);
      tm_ai.Elapsed += TimerAI;

      tm_perf = new System.Timers.Timer(1000);
      tm_perf.Elapsed += TimerPerf;
    }

    private Stopwatch stopwatch = Stopwatch.StartNew();
    public float Time { get { return stopwatch.ElapsedMilliseconds / 1000f; } }
    public float NextTime { get; private set; }
    public float GameTime { get; private set; }

    public bool IsLoading { get; private set; }
    public bool IsRunning { get; private set; }
    public float FPSLimit { get; private set; }
    public float RenderLowFPSLimit { get; private set; }
    public float LowFPSLimit { get; private set; }
    public float TimeSinceRender { get; set; }
    public float AddTime { get; set; }
    public bool IsPaused { get; set; }

    private Thread th_load { get; set; }
    private Thread th_tick { get; set; }
    private System.Timers.Timer tm_sound { get; set; }
    private System.Timers.Timer tm_perf { get; set; }
    private System.Timers.Timer tm_ai { get; set; }
    public int CurrentFPS { get; private set; }

    private bool isProcessingPerf = false;
    private bool isProcessingAI = false;

    /// <summary>
    /// Checks if the current FPS is below the LowFPSLimit
    /// </summary>
    /// <returns></returns>
    public bool IsLowFPS()
    {
      return CurrentFPS < LowFPSLimit;
    }

    /// <summary>
    /// Determines the modifiers to render culling distance to increase performance
    /// </summary>
    public float PerfCullModifier
    {
      get
      {
        return (CurrentFPS < RenderLowFPSLimit) ? (CurrentFPS / RenderLowFPSLimit < 0.5f) ? 0.5f : CurrentFPS / RenderLowFPSLimit : 1;
      }
    }

    public void StartLoad()
    {
      IsLoading = true;
    }

    public void Run()
    {
      IsRunning = true;
      IsLoading = false;
      th_tick.Start();
      tm_perf.Start();
    }

    public void Close()
    {
      IsRunning = false;
      IsLoading = false;
    }

    private void Tick()
    {
      Thread.CurrentThread.Name = "Main Loop";
      Thread.Sleep(1000);

      PerfManager.Instance().ClearPerf();

      // Pre-load
      th_load.Start();
      while (th_load.ThreadState == System.Threading.ThreadState.Running)
      {
        Engine.Instance().PreRender();
        Thread.Sleep(50);
      }

      // Initialize other threads/timers
      tm_sound.Start();
      tm_ai.Start();

      try
      {
        NextTime = Time;
        float tm = Time;
        while (IsRunning)
        {
          using (new PerfElement("tick_full"))
          {
            tm = Time; // Time can change within a single loop
            if (tm >= NextTime)
            {
              if (NextTime < tm - 10 / FPSLimit)
              {
                NextTime = tm + 1 / FPSLimit;
              }
              else
              {
                NextTime += 1 / FPSLimit;
              }
              using (new PerfElement("tick"))
              {
                TimeSinceRender = Engine.Instance().TVEngine.AccurateTimeElapsed() / 1000f;
                TimeSinceRender += AddTime;
                AddTime = 0;

                TickRender();
                TickProcess();
                TickCollision(); // can be decoupled if needed
                //TickAI(); // can be decoupled if needed
                if (!IsPaused)
                {
                  GameTime += TimeSinceRender;
                }

                CurrentFPS = Engine.Instance().TVEngine.GetFPS();
              }
            }
            else
            {
              Thread.Sleep((int)(1000 * (NextTime - tm)));
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (IsRunning)
        {
          IsPaused = true;
          string message = "An exception has been encountered!\n" + ex.Message + "\n\n" + ex.StackTrace;
          MessageBox.Show(message, Application.ProductName + " - Error Encountered!", MessageBoxButtons.OK);
        }
      }
      Engine.Instance().Dispose();
      Close();
    }

    private void TimerSound(object sender, ElapsedEventArgs e)
    {
      if (SoundManager.Instance().PendingUpdate)
      {
        TickSound();
      }
      if (!IsRunning)
      {
        tm_sound.Stop();
      }
    }

    private void TimerAI(object sender, ElapsedEventArgs e)
    {
      if (!isProcessingAI)
      {
        isProcessingAI = true;
        TickAI();
        isProcessingAI = false;
      }
      if (!IsRunning)
      {
        tm_ai.Stop();
      }
    }

    private void TimerPerf(object sender, ElapsedEventArgs e)
    {
      if (!isProcessingPerf)
      {
        isProcessingPerf = true;
        TickPerf();
        isProcessingPerf = false;
      }
      if (!IsRunning)
      {
        tm_perf.Stop();
      }
    }

    #region Tick Functions

    private void TickRender()
    {
      using (new PerfElement("tick_render"))
      {
        Engine.Instance().Render();
      }
    }

    private void TickProcess()
    {
      using (new PerfElement("tick_process_input"))
      {
        InputManager.Instance().ProcessInput();
      }

      using (new PerfElement("tick_process_actors"))
      {
        Engine.Instance().Process();
      }

      using (new PerfElement("tick_process_player"))
      {
        Engine.Instance().ProcessPlayer();
      }

      if (!IsPaused)
      {
        using (new PerfElement("tick_process_plannedactors"))
        {
          ActorFactory.Instance().ActivatePlanned();
        }

        using (new PerfElement("tick_process_scenario"))
        {
          GameScenarioManager.Instance().Update();
        }
      }

    }

    private void TickAI()
    {
      using (new PerfElement("tick_ai"))
      {
        Engine.Instance().ProcessAI();
      }
    }

    private void TickSound()
    {
      using (new PerfElement("tick_sound"))
      {
        SoundManager.Instance().Update();
      }
    }

    private void TickCollision()
    {
      using (new PerfElement("tick_collision"))
      {
        Engine.Instance().ProcessCollision();
      }
    }

    private void TickPerf()
    {
      using (new PerfElement("perf"))
      {
        PerfManager.Instance().PrintPerf();
      }
    }

    #endregion
  }
}


