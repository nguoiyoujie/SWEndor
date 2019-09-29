using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using SWEndor.Primitives;
using System.Threading;
using SWEndor.Primitives.Factories;
using SWEndor.Primitives.Collections;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;

namespace SWEndor
{
  public class PerfComparer : IComparer<PerfToken>
  {
    int IComparer<PerfToken>.Compare(PerfToken x, PerfToken y)
    {
      return x.Name.CompareTo(y.Name);
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
      pool = new ObjectPool<PerfElement>(() => new PerfElement(this, "<undefined>"), (p) => p.Reset());
      sbpool = new ObjectPool<StringBuilder>(() => new StringBuilder(128), (p) => p.Clear());
    }

    public static string PerfLogPath = Path.Combine(Globals.LogPath, @"perf.log");
    public string Report = string.Empty;
    private DateTime m_last_refresh_time = DateTime.Now;
    private CircularQueue<PerfToken> Queue;
    private Dictionary<string, PerfToken> Elements;
    private Dictionary<int, double> ThreadTimes;

    private Dictionary<int, List<PerfElement>> pt_current = new Dictionary<int, List<PerfElement>>();
    private ObjectPool<PerfElement> pool;
    private ObjectPool<StringBuilder> sbpool;

    private bool _enabled;
    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        if (_enabled != value)
        {
          if (value)
            Init();

          _enabled = value;

          if (!value)
            Deinit();
        }
      }
    }

    public void Init()
    {
      m_last_refresh_time = DateTime.Now;
      Queue = new CircularQueue<PerfToken>(100000, false);
      Elements = new Dictionary<string, PerfToken>();
      ThreadTimes = new Dictionary<int, double>();
      pt_current = new Dictionary<int, List<PerfElement>>();
    }

    public void Deinit()
    {
      //Queue = null;
      Elements.Clear();
      ThreadTimes.Clear();
      pt_current.Clear();
    }

    public PerfElement Create(string name)
    {
      if (Enabled)
      {
        PerfElement e = pool.GetNew();
        e.Name = name;
        List<PerfElement> list;
        int id = Thread.CurrentThread.ManagedThreadId;
        if (pt_current.TryGetValue(id, out list) && list.Count > 0)
        {
          StringBuilder sb = sbpool.GetNew();
          sb.Append(list[list.Count - 1].Name);
          sb.Append(delimiter);
          sb.Append(name);
          e.Name = sb.ToString();
          sbpool.Return(sb);
        }

        if (pt_current.ContainsKey(id))
          pt_current[id].Add(e);
        else
          pt_current.Add(id, new List<PerfElement> { e });
        return e;
      }
      else
        return null;
    }

    public void UpdatePerfElement(PerfElement element, double value)
    {
      if (Queue.Count < 100000)
        Queue.Enqueue(new PerfToken { Name = element.Name, Seconds = value });

      int id = Thread.CurrentThread.ManagedThreadId;
      if (pt_current.ContainsKey(id))
        pt_current[id].Remove(element);

      pool.Return(element);
    }

    public void ProcessQueue()
    {
      PerfToken p;
      int limit = 100000;
      while (Queue.Count > 0 && limit >= 0) //Queue.TryDequeue(out p) || limit < 0)
      {
        p = Queue.Dequeue();
        if (p.Name == null)
          break;

        limit--;
        PerfToken pt = Elements.GetOrDefault(p.Name);
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

      if (Enabled)
        Refresh();
    }

    private static char[] delimiter = new char[] { '`' };
    public void PrintPerf()
    {
      if (!Enabled)
        return;

      if (!Directory.Exists(Globals.LogPath))
        Directory.CreateDirectory(Globals.LogPath);

      try
      {
        double refresh_ms = (DateTime.Now - m_last_refresh_time).TotalMilliseconds;
        m_last_refresh_time = DateTime.Now;
        StringBuilder sb = sbpool.GetNew();
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
            double d = pt.TotalProcessorTime.TotalMilliseconds - ThreadTimes.GetOrDefault(pt.Id);
            sb.AppendLine("Thread {0:00000} {1,17} : {2:0.00}%".F(pt.Id, pt.ThreadState, d / refresh_ms * 100));
            ThreadTimes.Put(pt.Id, pt.TotalProcessorTime.TotalMilliseconds);
          }
        }

        sb.AppendLine("                                 [ Count] Total|s   Avg|ms  Peak|ms");
        newElements.Sort(new PerfComparer());
        foreach (PerfToken e in newElements)
        {
          string[] div = e.Name.Split(delimiter);
          string name = (div.Length > 0) ? new string('-', div.Length - 1) + div[div.Length - 1] : div[div.Length - 1];
          name = (name.Length > 30) ? name.Remove(27) + "..." : name;
          sb.AppendLine("{0,-30} : [{1,6}] {2,7:0.000}  {3,7:0.00}  {4,7:0.00}".F(
                          name
                        , e.Count
                        , e.Seconds
                        , e.Seconds / e.Count * 1000
                        , e.Peak * 1000));
        }

        string filepath = PerfLogPath;
        File.AppendAllText(filepath, sb.ToString());
        Report = sb.ToString();
        sbpool.Return(sb);
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
    private PerfManager mgr;
    private long m_timestamp;

    public PerfElement(PerfManager manager, string name)
    {
      Name = name;
      mgr = manager;
      m_timestamp = Stopwatch.GetTimestamp();
    }

    public void Reset()
    {
      Name = "<undefined>";
      m_timestamp = Stopwatch.GetTimestamp();
    }

    public void Dispose()
    {
      if (mgr.Enabled)
        mgr.UpdatePerfElement(this, Math.Max(0.0, Stopwatch.GetTimestamp() - m_timestamp) / Stopwatch.Frequency);
    }
  }
}
