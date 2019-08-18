using System;
using System.Collections.Generic;

namespace SWEndor.Primitives
{


  public class TimeCache<B1, B2, T>
  {
    public float Time { get; private set; }
    private Func<B1, B2, T> fn;
    private T val;

    public TimeCache(float time, Func<B1, B2, T> func, B1 p1, B2 p2)
    {
      fn = func;
      Time = time;
      val = fn(p1, p2);
    }

    public T Get(float time, B1 p1, B2 p2)
    {
      if (Time < time)
      {
        val = fn(p1, p2);
        Time = time;
      }
      return val;
    }
  }

  public class TimeCache<T>
  {
    public float Time { get; private set; }
    private Func<T> fn;
    private T val;

    public TimeCache(float time, Func<T> func)
    {
      fn = func;
      Time = time;
      val = fn();
    }

    public T Get(float time)
    {
      if (Time < time)
      {
        val = fn();
        Time = time;
      }
      return val;
    }
  }
}
