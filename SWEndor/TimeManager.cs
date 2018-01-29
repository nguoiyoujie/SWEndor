using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class TimeManager
  {
    private DateTime _stored_dt;
    private DateTime _next_dt;
    private double _delaysec = 5.0;

    public void Set(DateTime dt)
    {
      _stored_dt = dt;
      SetDelay(_delaysec);
    }

    public void SetNow()
    {
      _stored_dt = DateTime.Now;
      SetDelay(_delaysec);
    }

    public void SetDelay(double seconds)
    {
      if (_delaysec != seconds)
        _delaysec = seconds;
      _next_dt = _stored_dt.AddSeconds(seconds);
    }

    public double SecondsRemaining
    {
      get
      {
        DateTime now = DateTime.Now;
        return (_next_dt > now) ? (_next_dt - now).TotalSeconds : 0;
      }
    }

    public double MillisecondsRemaining
    {
      get
      {
        DateTime now = DateTime.Now;
        return (_next_dt > now) ? (_next_dt - now).TotalMilliseconds : 0;
      }
    }

    public double TicksRemaining
    {
      get
      {
        DateTime now = DateTime.Now;
        return (_next_dt > now) ? (_next_dt - now).Ticks : 0;
      }
    }

    public double SecondsElapsed
    {
      get
      {
        return (DateTime.Now - _stored_dt).TotalSeconds;
      }
    }

  }
}
