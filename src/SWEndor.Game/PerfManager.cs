using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Primrose.Primitives.Factories;
using Primrose.Primitives.Collections;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game
{
  public class PerfComparer : IComparer<PerfToken>
  {
    int IComparer<PerfToken>.Compare(PerfToken x, PerfToken y)
    {
      int result = x.Key.ThreadId.CompareTo(y.Key.ThreadId);
      if (result == 0) { result = x.Key.Name.CompareTo(y.Key.Name); }
      return result;
    }
  }

  public struct PerfToken
  {
    public PerfKey Key;
    public int Count;
    public double Seconds;
    public double Peak;
  }

  public struct PerfKey
  {
    public string Name;
    public int ThreadId;

    public PerfKey(string name, int threadId)
    {
      Name = name;
      ThreadId = threadId;
    }
  }

  public class PerfManager
  {
    public readonly Engine Engine;
    internal PerfManager(Engine engine)
    {
      Engine = engine;
      pool = new ObjectPool<PerfElement>(false, () => new PerfElement(this, "<undefined>", -1), (p) => p.Reset());
      sbpool = new ObjectPool<StringBuilder>(false, () => new StringBuilder(128), (p) => p.Clear());
    }

    public static string PerfLogPath = Path.Combine(Globals.LogPath, @"perf.log");
    public string Report = string.Empty;
    private DateTime m_last_refresh_time = DateTime.Now;
    private CircularQueue<PerfToken> Queue;
    private Dictionary<PerfKey, PerfToken> Elements;
    private Dictionary<int, double> ThreadTimes;

    private Dictionary<int, List<PerfElement>> pt_current = new Dictionary<int, List<PerfElement>>();
    private readonly ObjectPool<PerfElement> pool;
    private readonly ObjectPool<StringBuilder> sbpool;

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
      Elements = new Dictionary<PerfKey, PerfToken>();
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
        int id = Thread.CurrentThread.ManagedThreadId;
        if (pt_current.TryGetValue(id, out List<PerfElement> list) && list.Count > 0)
        {
          StringBuilder sb = sbpool.GetNew();
          sb.Append(list[list.Count - 1].Name);
          sb.Append(delimiter);
          sb.Append(name);
          e.Name = sb.ToString();
          e.ThreadId = id;
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
        Queue.Enqueue(new PerfToken { Key = new PerfKey(element.Name, element.ThreadId), Seconds = value });

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
        if (p.Key.Name == null)
          break;

        limit--;
        PerfToken pt = Elements.GetOrDefault(p.Key);
        if (pt.Key.Name == null)
        {
          Elements.Put(p.Key, new PerfToken { Key = p.Key, Count = 1, Seconds = p.Seconds, Peak = p.Seconds });
        }
        else
        {
          double peak = (pt.Peak < p.Seconds) ? p.Seconds : pt.Peak;
          double seconds = pt.Seconds + p.Seconds;
          int count = ++pt.Count;

          Elements.Put(p.Key, new PerfToken { Key = p.Key, Count = count, Seconds = seconds, Peak = peak });
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

    private static readonly char[] delimiter = new char[] { '`' };
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
        sb.AppendLine("{0,35} : [{1,4:0}ms] {2:s}".F("Sampling Time", refresh_ms, m_last_refresh_time));
        sb.AppendLine("{0,35} : {1}".F("FPS", Engine.Game.CurrentFPS));
        sb.AppendLine("{0,35} : {1}".F("Actors", Engine.ActorFactory.GetActorCount()));

        List<PerfToken> newElements = new List<PerfToken>(Elements.Values);

        // ---------- PROCESS THREAD
        foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
        {
          if (pt.ThreadState != System.Diagnostics.ThreadState.Terminated && pt.ThreadState != System.Diagnostics.ThreadState.Wait)
          {
            double d = pt.TotalProcessorTime.TotalMilliseconds - ThreadTimes.GetOrDefault(pt.Id);
            sb.AppendLine("Thread {0:00000} {1,22} : {2:0.00}%".F(pt.Id, pt.ThreadState, d / refresh_ms * 100));
            ThreadTimes.Put(pt.Id, pt.TotalProcessorTime.TotalMilliseconds);
          }
        }

        sb.AppendLine("                                      [ Count] Total|s   Avg|ms  Peak|ms");
        newElements.Sort(new PerfComparer());
        foreach (PerfToken e in newElements)
        {
          string[] div = e.Key.Name.Split(delimiter);
          string name = (div.Length > 0) ? new string('-', div.Length - 1) + div[div.Length - 1] : div[div.Length - 1];
          name = (name.Length > 30) ? name.Remove(27) + "..." : name;
          sb.AppendLine("[{0,3}] {1,-29} : [{2,6}] {3,7:0.000}  {4,7:0.00}  {5,7:0.00}".F(
                        e.Key.ThreadId
                        , name
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
    public int ThreadId;
    private readonly PerfManager mgr;
    private long m_timestamp;

    public PerfElement(PerfManager manager, string name, int threadId)
    {
      Name = name;
      ThreadId = threadId;
      mgr = manager;
      m_timestamp = Stopwatch.GetTimestamp();
    }

    public void Reset()
    {
      Name = "<undefined>";
      ThreadId = -1;
      m_timestamp = Stopwatch.GetTimestamp();
    }

    public void Dispose()
    {
      if (mgr.Enabled)
        mgr.UpdatePerfElement(this, Math.Max(0.0, Stopwatch.GetTimestamp() - m_timestamp) / Stopwatch.Frequency);
    }
  }
}
