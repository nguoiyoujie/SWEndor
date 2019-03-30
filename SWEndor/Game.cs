using SWEndor.Actors;
using SWEndor.Input;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Sound;
using SWEndor.UI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
      TimeSinceRender = 1;
      th_load = new Thread(new ThreadStart(Engine.Instance().Load));


      tm_sound = new System.Timers.Timer(30);
      tm_sound.Elapsed += TimerSound;

      tm_ai = new System.Timers.Timer(30);
      tm_ai.Elapsed += TimerAI;

      tm_collision = new System.Timers.Timer(30);
      tm_collision.Elapsed += TimerCollision;

      tm_render = new System.Timers.Timer(30);
      tm_render.Elapsed += TimerRender;

      tm_process = new System.Timers.Timer(30);
      tm_process.Elapsed += TimerProcess;

      tm_perf = new System.Timers.Timer(1000);
      tm_perf.Elapsed += TimerPerf;
    }

    public TimeControl TimeControl = new TimeControl();

    public float GameTime { get; private set; }

    public bool IsLoading { get; private set; }
    public bool IsRunning { get; private set; }
    public float TimeSinceRender { get; set; }
    public float AddTime { get; set; }
    public bool IsPaused { get; set; }

    private Thread th_load { get; set; }
    private Thread th_tick { get; set; }
    
    private System.Timers.Timer tm_sound { get; set; }
    private System.Timers.Timer tm_perf { get; set; }
    private System.Timers.Timer tm_ai { get; set; }
    private System.Timers.Timer tm_collision { get; set; }
    private System.Timers.Timer tm_render { get; set; }
    private System.Timers.Timer tm_process { get; set; }
    
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
        return (CurrentFPS < TimeControl.PerformanceSavingFPS) ? (CurrentFPS / TimeControl.PerformanceSavingFPS < 0.5f) ? 0.5f : CurrentFPS / TimeControl.PerformanceSavingFPS : 1;
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
      ////th_tick.Start();
      tm_perf.Start();
      //Task.Factory.StartNew(new Action(TaskPerf)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      Task.Factory.StartNew(new Action(Tick)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      //GameForm.Instance().Show();
      //Tick();
    }

    public void Close()
    {
      if (IsRunning)
      {
        IsRunning = false;
        IsLoading = false;
        GameForm.Instance().Exit();
      }
    }

    private void Tick()
    {
      Thread.CurrentThread.Name = "Main Loop";
      Thread.Sleep(1000);

      PerfManager.Instance().ClearPerf();

      //float lastelapsed = 0;
      //float elapsed = 0;

      // Pre-load
      th_load.Start();
      while (th_load.ThreadState == System.Threading.ThreadState.Running)
      {
        Engine.Instance().PreRender();
        TimeControl.Update();
        TimeControl.Wait();

        using (new PerfElement("tick_process_input"))
          InputManager.Instance().ClearInput();
      }

      // Initialize other threads/timers
      if (septhread_sound)
        tm_sound.Start();
        //Task.Factory.StartNew(new Action(TaskSound)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      if (septhread_ai)
        tm_ai.Start();
      //Task.Factory.StartNew(new Action(TaskAI)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      if (septhread_collision)
        tm_collision.Start();
      //Task.Factory.StartNew(new Action(TaskCollision)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      if (septhread_render)
        tm_render.Start();
      //Task.Factory.StartNew(new Action(TaskRender)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      if (septhread_process)
        tm_process.Start();
      //Task.Factory.StartNew(new Action(TaskProcess)).ContinueWith(new Action<Task>(t => GenerateFatalError(t.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      try
      {
        while (IsRunning)
        {
          try
          {
            using (new PerfElement("tick"))
            {
              TimeControl.Update();
              //lastelapsed = elapsed;
              TimeSinceRender = (1f / TimeControl.TargetFPS) * TimeControl.SpeedModifier; //TimeControl.WorldInterval;
              TimeControl.AddTime(AddTime);
              TimeSinceRender += AddTime;
              AddTime = 0;

              if (!septhread_sound)
                TickSound();

              if (!septhread_collision)
                TickCollision();

              if (!septhread_ai)
                TickAI();

              if (!septhread_render)
                TickRender();

              if (!septhread_process)
                TickProcess();

              if (!IsPaused)
                GameTime += TimeSinceRender;

              //CurrentFPS = Engine.Instance().TVEngine.GetFPS();
              //TimeControl.Wait();
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
        string errorfilename = @"error.txt";
        Globals.GenerateErrLog(ex, errorfilename);
        MessageBox.Show(string.Format("Fatal Error occurred during runtime. Please see " + errorfilename + " in the /Log folder for the error message."
                      , Application.ProductName + " - Error Encountered!"
                      , MessageBoxButtons.OK));
        return;
      }

      Engine.Instance().Dispose();
      Close();
    }

    private void GenerateFatalError(Exception ex)
    {
      // Replace this block to print this on file!
      if (IsRunning)
      {
        Screen2D.Instance().CurrentPage = new UIPage_FatalError(ex);
        Screen2D.Instance().ShowPage = true;
        IsPaused = true;
      }
    }

    private void TimerSound(object sender, ElapsedEventArgs e)
    {
      TickSound();
      if (!IsRunning)
        tm_sound.Stop();
    }

    private void TimerAI(object sender, ElapsedEventArgs e)
    {
      TickAI();
      if (!IsRunning)
        tm_ai.Stop();
    }

    private void TimerCollision(object sender, ElapsedEventArgs e)
    {
      TickCollision();
      if (!IsRunning)
        tm_collision.Stop();
    }

    private void TimerRender(object sender, ElapsedEventArgs e)
    {
      TickRender();
      if (!IsRunning)
        tm_render.Stop();
    }

    private void TimerProcess(object sender, ElapsedEventArgs e)
    {
      TickProcess();
      if (!IsRunning)
        tm_process.Stop();
    }

    private void TimerPerf(object sender, ElapsedEventArgs e)
    {
      TickPerf();
      if (!IsRunning)
        tm_perf.Stop();
    }

    #region Tick Functions

    private void TickRender()
    {
      if (!isProcessingRender)
      {
        isProcessingRender = true;
        //using (new PerfElement("tick_render_playercamera"))
        //  Engine.Instance().ProcessPlayerCamera();

        using (new PerfElement("tick_render"))
          Engine.Instance().Render();

        isProcessingRender = false;
      }
    }

    private void TickProcess()
    {
      if (!isProcessingProcess)
      {
        isProcessingProcess = true;
        
        using (new PerfElement("tick_process_input"))
          InputManager.Instance().ProcessInput();

        using (new PerfElement("tick_process_actors"))
          Engine.Instance().Process();

        using (new PerfElement("tick_process_player"))
          Engine.Instance().ProcessPlayer();

        using (new PerfElement("tick_page"))
          Screen2D.Instance().CurrentPage?.Tick();

        if (!IsPaused)
        {
          using (new PerfElement("tick_process_plannedactors"))
            ActorFactory.Instance().ActivatePlanned();

          using (new PerfElement("tick_process_scenario"))
            GameScenarioManager.Instance().Update();
        }

        using (new PerfElement("tick_render_playercamera"))
          Engine.Instance().ProcessPlayerCamera();

        isProcessingProcess = false;
      }
    }

    private void TickAI()
    {
      if (!isProcessingAI)
      {
        isProcessingAI = true;
        using (new PerfElement("tick_ai"))
          Engine.Instance().ProcessAI();
        isProcessingAI = false;
      }
    }

    private void TickSound()
    {
      if (SoundManager.Instance().PendingUpdate)
        using (new PerfElement("tick_sound"))
          SoundManager.Instance().Update();
    }

    private void TickCollision()
    {
      if (!isProcessingCollision)
      {
        isProcessingCollision = true;
        using (new PerfElement("tick_collision"))
          Engine.Instance().ProcessCollision();
        isProcessingCollision = false;
      }
    }

    private void TickPerf()
    {
      if (!isProcessingPerf)
      {
        isProcessingPerf = true;
        using (new PerfElement("perf"))
          PerfManager.Instance().PrintPerf();
        Thread.Sleep(1000);
        isProcessingPerf = false;
      }
    }
    #endregion
  }
}


