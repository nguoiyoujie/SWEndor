using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;


namespace SWEndor
{
  public class PerfToken
  {
    public int Count;
    public double Seconds;
    public double Peak;
  }

  public class PerfManager
  {
    private static PerfManager m_instance;
    public static PerfManager Instance()
    {
      if (m_instance == null)
      {
        m_instance = new PerfManager();
      }
      return m_instance;
    }
    private PerfManager() { }

    private DateTime m_last_refresh_time = DateTime.Now;
    private ThreadSafeDictionary<string, PerfToken> Elements = new ThreadSafeDictionary<string, PerfToken>();
    private ThreadSafeDictionary<int, double> ThreadTimes = new ThreadSafeDictionary<int, double>();

    public void UpdatePerfElement(string name, double value)
    {
      PerfToken pt = Elements.GetItem(name);
      if (pt == null)
      {
        Elements.AddorUpdateItem(name, new PerfToken { Count = 1, Seconds = value, Peak = value });
      }
      else
      {
        pt.Count++;
        pt.Seconds += value;
        if (pt.Peak < value)
          pt.Peak = value;
      }
    }

    private void Refresh()
    {
      Elements.ClearList();
    }

    public void ClearPerf()
    {
      string filepath = Path.Combine(Globals.LogPath, @"perf.log");
      if (File.Exists(filepath))
        File.Delete(filepath);

      Refresh();
    }

    public void PrintPerf()
    {
      if (!Settings.ShowPerformance)
        return;

      if (!Directory.Exists(Globals.LogPath))
        Directory.CreateDirectory(Globals.LogPath);

      try
      {
        double refresh_ms = (DateTime.Now - m_last_refresh_time).TotalMilliseconds;
        m_last_refresh_time = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(string.Format("{0,30} : [{1,4:0}ms] {2:s}", "Sampling Time", refresh_ms, m_last_refresh_time));
        sb.AppendLine(string.Format("{0,30} : {1}", "FPS", Game.Instance().CurrentFPS));
        sb.AppendLine(string.Format("{0,30} : {1}", "Actors", ActorFactory.Instance().GetActorList().Length));

        SortedDictionary<string, PerfToken> newElements = new SortedDictionary<string, PerfToken>(Elements.GetList());

        // ---------- PROCESS THREAD
        foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
        {
          if (pt.ThreadState != System.Diagnostics.ThreadState.Terminated && pt.ThreadState != System.Diagnostics.ThreadState.Wait)
          {
            double d = pt.TotalProcessorTime.TotalMilliseconds - ThreadTimes.GetItem(pt.Id);
            sb.AppendLine(string.Format("Thread {0:00000} {1,17} : {2:0.00}%", pt.Id, pt.ThreadState, d / refresh_ms * 100));
            ThreadTimes.AddorUpdateItem(pt.Id, pt.TotalProcessorTime.TotalMilliseconds);
          }
        }

        sb.AppendLine(string.Format("{0,-30}   [{1,6}] {2,7}  {3,7}  {4,7}", "", "Count", "Total|s", "Avg|ms", "Peak|ms"));
        foreach (KeyValuePair<string, PerfToken> e in newElements)
        {
          sb.AppendLine(string.Format("{0,-30} : [{1,6}] {2,7:0.000}  {3,7:0.00}  {4,7:0.00}", e.Key, e.Value.Count, e.Value.Seconds, e.Value.Seconds / e.Value.Count * 1000, e.Value.Peak * 1000));
        }


        string filepath = Path.Combine(Globals.LogPath, @"perf.log");
        File.AppendAllText(filepath, sb.ToString());
        Screen2D.Instance().perftext = sb.ToString();
        Refresh();
      }
      catch
      {
      }
    }
  }

  public class PerfElement : IDisposable
  {
    public string Name;
    private long m_timestamp;

    public PerfElement(string name)
    {
      Name = name;
      m_timestamp = Stopwatch.GetTimestamp();
    }

    public void Dispose()
    {
      if (Settings.ShowPerformance)
        PerfManager.Instance().UpdatePerfElement(Name, Math.Max(0.0, Stopwatch.GetTimestamp() - m_timestamp) / Stopwatch.Frequency);
    }
  }
}
