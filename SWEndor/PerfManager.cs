﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;
using SWEndor.Primitives;

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
    public readonly Engine Engine;
    internal PerfManager(Engine engine)
    {
      Engine = engine;
    }

    public static string PerfLogPath = Path.Combine(Globals.LogPath, @"perf.log");
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
      if (File.Exists(PerfLogPath))
        File.Delete(PerfLogPath);

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
        sb.AppendLine("{0,30} : [{1,4:0}ms] {2:s}".F("Sampling Time", refresh_ms, m_last_refresh_time));
        sb.AppendLine("{0,30} : {1}".F("FPS", Engine.Game.CurrentFPS));
        sb.AppendLine("{0,30} : {1}".F("Actors", Engine.ActorFactory.GetActorCount()));

        List<PerfToken> newElements = new List<PerfToken>(Elements.Values);

        // ---------- PROCESS THREAD
        foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
        {
          if (pt.ThreadState != System.Diagnostics.ThreadState.Terminated && pt.ThreadState != System.Diagnostics.ThreadState.Wait)
          {
            double d = pt.TotalProcessorTime.TotalMilliseconds - ThreadTimes.Get(pt.Id);
            sb.AppendLine("Thread {0:00000} {1,17} : {2:0.00}%".F(pt.Id, pt.ThreadState, d / refresh_ms * 100));
            ThreadTimes.Put(pt.Id, pt.TotalProcessorTime.TotalMilliseconds);
          }
        }

        sb.AppendLine("{0,-30}   [{1,6}] {2,7}  {3,7}  {4,7}".F("", "Count", "Total|s", "Avg|ms", "Peak|ms"));
        newElements.Sort(new PerfComparer());
        foreach (PerfToken e in newElements)
        {
          sb.AppendLine("{0,-30} : [{1,6}] {2,7:0.000}  {3,7:0.00}  {4,7:0.00}".F(
                          (e.Name.Length > 30) ? e.Name.Remove(27) + "..." : e.Name
                        , e.Count
                        , e.Seconds
                        , e.Seconds / e.Count * 1000
                        , e.Peak * 1000));
        }

        string filepath = PerfLogPath;
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
        Globals.Engine.PerfManager.UpdatePerfElement(Name, Math.Max(0.0, Stopwatch.GetTimestamp() - m_timestamp) / Stopwatch.Frequency);
    }
  }
}
