using System;
using System.Threading;

namespace Primrose.Primitives
{
  /// <summary>
  /// Provides implementation for maintaining a reference counter for explicit management
  /// </summary>
  public static class ScopeCounters
  {
    private static ScopeGlobalCounter[] _cache;
    static ScopeCounters()
    {
      _cache = new ScopeGlobalCounter[byte.MaxValue + 1];
      for (byte i = byte.MaxValue; ; i--)
      {
        _cache[i] = new ScopeGlobalCounter(i);
        if (i == 0)
          break;
      }
    }

    /// <summary>Gets the reference count of a global scope</summary>
    /// <param name="global">The global scope</param>
    /// <returns>The reference count of the scope</returns>
    public static int Get(byte global) { return _cache[global]._count; }

    /// <summary>Gets the reference count of a local scope</summary>
    /// <param name="scope">The local scope</param>
    /// <returns>The reference count of the scope</returns>
    public static int Get(ScopeCounter scope) { return scope._count; }

    /// <summary>Increments the reference count of a global scope, then returns the updated scope</summary>
    /// <param name="global">The global scope</param>
    /// <returns>The updated scope</returns>
    public static ScopeGlobalCounter Acquire(byte global) { Interlocked.Increment(ref _cache[global]._count); return _cache[global]; }

    /// <summary>Increments the reference count of a local scope, then returns the updated scope</summary>
    /// <param name="scope">The local scope</param>
    /// <returns>The updated scope</returns>
    public static ScopeCounter Acquire(ScopeCounter scope) { Interlocked.Increment(ref scope._count); return scope; }

    /// <summary>Resets the reference count of a global scope to zero</summary>
    /// <param name="global">The global scope</param>
    public static void Reset(byte global) { Interlocked.Exchange(ref _cache[global]._count, 0); }

    /// <summary>Resets the reference count of a global scope to zero</summary>
    /// <param name="scope">The local scope</param>
    public static void Reset(ScopeCounter scope) { Interlocked.Exchange(ref scope._count, 0); }

    /// <summary>Decrements the reference count of a global scope</summary>
    /// <param name="global">The global scope</param>
    public static void ReleaseOne(byte global) { Interlocked.Decrement(ref _cache[global]._count); }

    /// <summary>Decrements the reference count of a local scope</summary>
    /// <param name="scope">The local scope</param>
    private static void ReleaseOne(ScopeCounter scope) { Interlocked.Decrement(ref scope._count); }

    /// <summary>Decrements the reference count of a global scope</summary>
    /// <param name="scope">The global scope</param>
    private static void ReleaseOne(ScopeGlobalCounter scope) { Interlocked.Decrement(ref scope._count); }

    /// <summary>Increments the reference count of a global scope only if the count is zero</summary>
    /// <param name="global">The global scope</param>
    /// <returns>True if acquired, false otherwise</returns>
    public static bool AcquireIfZero(byte global) { return Interlocked.CompareExchange(ref _cache[global]._count, 1, 0) == 0; }

    /// <summary>Increments the reference count of a local scope only if the count is zero</summary>
    /// <param name="scope">The local scope</param>
    /// <returns>True if acquired, false otherwise</returns>
    public static bool AcquireIfZero(ScopeCounter scope) { return Interlocked.CompareExchange(ref scope._count, 1, 0) == 0; }

    /// <summary>Performs a spinlock until the reference count of a global scope is zero, then increments the reference count</summary>
    /// <param name="global">The global scope</param>
    /// <returns>The updated scope</returns>
    public static ScopeGlobalCounter AcquireWhenZero(byte global) { while (!(Interlocked.CompareExchange(ref _cache[global]._count, 1, 0) == 0)) ; return _cache[global]; }

    /// <summary>Performs a spinlock until the reference count of a local scope is zero, then increments the reference count</summary>
    /// <param name="scope">The local scope</param>
    /// <returns>The updated scope</returns>
    public static ScopeCounter AcquireWhenZero(ScopeCounter scope) { while (!(Interlocked.CompareExchange(ref scope._count, 1, 0) == 0)) ; return scope; }

    /// <summary>Returns if the reference count of a scope is zero</summary>
    /// <param name="t1">The scope to check</param>
    /// <returns>True if the reference count is zero, false otherwise</returns>
    public static bool IsZero(ScopeCounter t1) { return Get(t1) == 0; }

    /// <summary>Returns if the reference count of a scope is zero</summary>
    /// <param name="t1">The scope to check</param>
    /// <returns>True if the reference count is zero, false otherwise</returns>
    public static bool IsZero(byte t1) { return Get(t1) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(ScopeCounter t1, ScopeCounter t2) { return Get(t1) == 0 && Get(t2) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(ScopeCounter t1, byte t2) { return Get(t1) == 0 && Get(t2) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(byte t1, byte t2) { return Get(t1) == 0 && Get(t2) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(ScopeCounter t1, ScopeCounter t2, ScopeCounter t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(ScopeCounter t1, ScopeCounter t2, byte t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(ScopeCounter t1, byte t2, byte t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(byte t1, byte t2, byte t3) { return Get(t1) == 0 && Get(t2) == 0 && Get(t3) == 0; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="ts">The list of scopes to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(params ScopeCounter[] ts) { for (int i = ts.Length - 1; ; i--) if (Get(ts[i]) != 0) return false; else if (i == 0) return true; }

    /// <summary>Returns if the reference count of all scopes is zero</summary>
    /// <param name="ts">The list of scopes to check</param>
    /// <returns>True if all reference counts are zero, false otherwise</returns>
    public static bool IsZero(params byte[] ts) { for (int i = ts.Length - 1; ; i--) if (Get(ts[i]) != 0) return false; else if (i == 0) return true; }

    /// <summary>Performs a spinlock until the reference count of a global scope is zero.</summary>
    /// <param name="t1">The scope to check</param>
    public static void WaitForZero(ScopeCounter t1) { while (!(IsZero(t1))) ; }

    /// <summary>Performs a spinlock until the reference count of a global scope is zero.</summary>
    /// <param name="t1">The scope to check</param>
    public static void WaitForZero(byte t1) { while (!(IsZero(t1))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    public static void WaitForZero(ScopeCounter t1, ScopeCounter t2) { while (!(IsZero(t1, t2))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    public static void WaitForZero(ScopeCounter t1, byte t2) { while (!(IsZero(t1, t2))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    public static void WaitForZero(byte t1, byte t2) { while (!(IsZero(t1, t2))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    public static void WaitForZero(ScopeCounter t1, ScopeCounter t2, ScopeCounter t3) { while (!(IsZero(t1, t2, t3))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    public static void WaitForZero(ScopeCounter t1, ScopeCounter t2, byte t3) { while (!(IsZero(t1, t2, t3))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    public static void WaitForZero(ScopeCounter t1, byte t2, byte t3) { while (!(IsZero(t1, t2, t3))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="t1">The first scope to check</param>
    /// <param name="t2">The second scope to check</param>
    /// <param name="t3">The third scope to check</param>
    public static void WaitForZero(byte t1, byte t2, byte t3) { while (!(IsZero(t1, t2, t3))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="ts">The list of scopes to check</param>
    public static void WaitForZero(params ScopeCounter[] ts) { while (!(IsZero(ts))) ; }

    /// <summary>Performs a spinlock until all reference counts are zero.</summary>
    /// <param name="ts">The list of scopes to check</param>
    public static void WaitForZero(params byte[] ts) { while (!(IsZero(ts))) ; }

    /// <summary>
    /// Represents a global scope reference counter
    /// </summary>
    public class ScopeGlobalCounter : IDisposable
    {
      /// <summary>The global scope reference value</summary>
      public readonly byte Token;
      internal int _count;

      /// <summary>The global scope reference count</summary>
      public int Count { get { return _count; } }
      internal ScopeGlobalCounter(byte token) { Token = token; }

      /// <summary>Decrements the reference count</summary>
      public void Dispose() { ReleaseOne(this); }
    }

    /// <summary>
    /// Represents a local scope reference counter
    /// </summary>
    public class ScopeCounter : IDisposable
    {
      internal int _count;

      /// <summary>The global scope reference count</summary>
      public int Count { get { return _count; } }

      /// <summary>Creates a scope reference counter</summary>
      public ScopeCounter() { }

      /// <summary>Decrements the reference count</summary>
      public void Dispose() { ReleaseOne(this); }
    }
  }
}
