using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using SWEndor.Actors;
using System.Collections.Concurrent;

namespace SWEndor
{
  public class PerfComparer : IComparer<PerfToken>
  {
    int IComparer<PerfToken>.Compare(PerfToken x, PerfToken y)
    {
      return (int)(y.Seconds * 1000 - x.Seconds * 1000);
    }
  }

  public struct PerfToken
  {
    public string Name;
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

    public static string Report = "";
    private DateTime m_last_refresh_time = DateTime.Now;
    private ConcurrentQueue<PerfToken> Queue = new ConcurrentQueue<PerfToken>();
    private ThreadSafeDictionary<string, PerfToken> Elements = new ThreadSafeDictionary<string, PerfToken>();
    private ThreadSafeDictionary<int, double> ThreadTimes = new ThreadSafeDictionary<int, double>();

    public void UpdatePerfElement(string name, double value)
    {
      if (Queue.Count < 100000)
        Queue.Enqueue(new PerfToken { Name = name, Seconds = value });
    }

    public void ProcessQueue()
    {
      PerfToken p;
      int limit = 100000;
      while (Queue.TryDequeue(out p) || limit < 0)
      {
        if (p.Name == null)
          break;

          limit--;
        PerfToken pt = Elements.Get(p.Name);
        if (pt.Name == null)
        {
          Elements.Put(p.Name, new PerfToken { Name = p.Name, Count = 1, Seconds = p.Seconds, Peak = p.Seconds });
        }
        else
        {
          double peak = (pt.Peak < p.Seconds) ? p.Seconds : pt.Peak;
          double seconds = pt.Seconds + p.Seconds;
          int count = ++pt.Count;

          Elements.Put(p.Name, new PerfToken { Name = p.Name, Count = count, Seconds = seconds, Peak = peak });
        }
      }
    }

    private void Refresh()
    {
      Elements.Clear();
      ProcessQueue();
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
        sb.AppendLine(string.Format("{0,30} : {1}", "Actors", ActorInfo.Factory.GetActorCount()));

        List<PerfToken> newElements = new List<PerfToken>(Elements.GetValues());

        // ---------- PROCESS THREAD
        foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
        {
          if (pt.ThreadState != System.Diagnostics.ThreadState.Terminated && pt.ThreadState != System.Diagnostics.ThreadState.Wait)
          {
            double d = pt.TotalProcessorTime.TotalMilliseconds - ThreadTimes.Get(pt.Id);
            sb.AppendLine(string.Format("Thread {0:00000} {1,17} : {2:0.00}%", pt.Id, pt.ThreadState, d / refresh_ms * 100));
            ThreadTimes.Put(pt.Id, pt.TotalProcessorTime.TotalMilliseconds);
          }
        }

        sb.AppendLine(string.Format("{0,-30}   [{1,6}] {2,7}  {3,7}  {4,7}", "", "Count", "Total|s", "Avg|ms", "Peak|ms"));
        newElements.Sort(new PerfComparer());
        foreach (PerfToken e in newElements)
        {
          sb.AppendLine(string.Format("{0,-30} : [{1,6}] {2,7:0.000}  {3,7:0.00}  {4,7:0.00}"
                        , (e.Name.Length > 30) ? e.Name.Remove(27) + "..." : e.Name
                        , e.Count
                        , e.Seconds
                        , e.Seconds / e.Count * 1000
                        , e.Peak * 1000));
        }

        string filepath = Path.Combine(Globals.LogPath, @"perf.log");
        File.AppendAllText(filepath, sb.ToString());
        Report = sb.ToString();
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
