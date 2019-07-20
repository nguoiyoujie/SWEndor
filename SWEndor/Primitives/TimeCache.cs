using System;

namespace SWEndor.Primitives
{
  public struct TimeCache<T>
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

    public T Get<B>(float time, Func<B,T> func, B p1)
    {
      if (Time < time)
      {
        val = func(p1);
        Time = time;
      }
      return val;
    }

    public T Get<B1, B2>(float time, Func<B1, B2, T> func, B1 p1, B2 p2)
    {
      if (Time < time)
      {
        val = func(p1, p2);
        Time = time;
      }
      return val;
    }
  }
}
