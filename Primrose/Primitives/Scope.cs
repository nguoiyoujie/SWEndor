using System;
using System.Threading;

namespace SWEndor.Primitives
{
  public static class ScopeGlobals
  {
    public const byte GLOBAL_TVSCENE = 113;
  }

  public static class ScopeCounterManager
  {
    private static ScopeGlobalCounter[] _cache;
    static ScopeCounterManager()
    {
      _cache = new ScopeGlobalCounter[byte.MaxValue + 1];
      for (byte i = byte.MaxValue; ; i--)
      {
        _cache[i] = new ScopeGlobalCounter(i);
        if (i == 0)
          break;
      }
    }

    public static int Get(byte global) { return _cache[global]._count; }
    public static int Get(ScopeCounter scope) { return scope._count; }

    public static ScopeGlobalCounter Acquire(byte global) { Interlocked.Increment(ref _cache[global]._count); return _cache[global]; }
    public static ScopeCounter Acquire(ScopeCounter scope) { Interlocked.Increment(ref scope._count); return scope; }

    public static void Reset(byte global) { Interlocked.Exchange(ref _cache[global]._count, 0); }
    public static void Reset(ScopeCounter scope) { Interlocked.Exchange(ref scope._count, 0); }

    public static void ReleaseOne(byte global) { Interlocked.Decrement(ref _cache[global]._count); }
    private static void ReleaseOne(ScopeCounter scope) { Interlocked.Decrement(ref scope._count); }
    private static void ReleaseOne(ScopeGlobalCounter scope) { Interlocked.Decrement(ref scope._count); }

    public static bool AcquireIfZero(byte global) { return Interlocked.CompareExchange(ref _cache[global]._count, 1, 0) == 0; }
    public static bool AcquireIfZero(ScopeCounter scope) { return Interlocked.CompareExchange(ref scope._count, 1, 0) == 0; }

    public static ScopeGlobalCounter AcquireWhenZero(byte global) { while (!(Interlocked.CompareExchange(ref _cache[global]._count, 1, 0) == 0)) ; return _cache[global]; }
    public static ScopeCounter AcquireWhenZero(ScopeCounter scope) {  while (!(Interlocked.CompareExchange(ref scope._count, 1, 0) == 0)) ; return scope; }

    #region IsZero / WaitForZero
    public static bool IsZero(ScopeCounter t1) { return Get(t1) == 0; }
    public static bool IsZero(byte t1) { return Get(t1) == 0; }
    public static bool IsZero(ScopeCounter t1, ScopeCounter t2) { return Get(t1) == 0 && Get(t2) == 0; }
    public static bool IsZero(ScopeCounter t1, byte t2) { return Get(t1) == 0 && Get(t2) == 0; }
    public static bool IsZero(byte t1, byte t2) { return Get(t1) == 0 && Get(t2) == 0; }
    public static bool IsZero(ScopeCounter t1, ScopeCounter t2, ScopeCounter t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }
    public static bool IsZero(ScopeCounter t1, ScopeCounter t2, byte t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }
    public static bool IsZero(ScopeCounter t1, byte t2, byte t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }
    public static bool IsZero(byte t1, byte t2, byte t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }
    public static bool IsZero(params ScopeCounter[] ts) { for (int i = ts.Length - 1; ; i--) if (Get(ts[i]) != 0) return false; else if (i == 0) return true; }
    public static bool IsZero(params byte[] ts) { for (int i = ts.Length - 1; ; i--) if (Get(ts[i]) != 0) return false; else if (i == 0) return true; }

    public static void WaitForZero(ScopeCounter t1) {  while (!(IsZero(t1))); }
    public static void WaitForZero(byte t1) {  while (!(IsZero(t1))); }
    public static void WaitForZero(ScopeCounter t1, ScopeCounter t2) {  while (!(IsZero(t1, t2))); }
    public static void WaitForZero(ScopeCounter t1, byte t2) {  while (!(IsZero(t1, t2))); }
    public static void WaitForZero(byte t1, byte t2) {  while (!(IsZero(t1, t2))); }
    public static void WaitForZero(ScopeCounter t1, ScopeCounter t2, ScopeCounter t3) {  while (!(IsZero(t1, t2, t3))); }
    public static void WaitForZero(ScopeCounter t1, ScopeCounter t2, byte t3) {  while (!(IsZero(t1, t2, t3))); }
    public static void WaitForZero(ScopeCounter t1, byte t2, byte t3) {  while (!(IsZero(t1, t2, t3))); }
    public static void WaitForZero(byte t1, byte t2, byte t3) {  while (!(IsZero(t1, t2, t3))); }
    public static void WaitForZero(params ScopeCounter[] ts) {  while (!(IsZero(ts))); }
    public static void WaitForZero(params byte[] ts) {  while (!(IsZero(ts))); }
    #endregion

    public class ScopeGlobalCounter : IDisposable
    {
      public readonly byte Token;
      internal int _count;
      public int Count { get { return _count; } }
      internal ScopeGlobalCounter(byte token) { Token = token; }
      public void Dispose() { ReleaseOne(this); }
    }

    public class ScopeCounter : IDisposable
    {
      internal int _count;
      public int Count { get { return _count; } }
      public ScopeCounter() { }
      public void Dispose() { ReleaseOne(this); }
    }
  }
}
