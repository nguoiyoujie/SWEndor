using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace Primrose.Primitives
{
  /// <summary>
  /// Provides a basic cache functionality in a dictionary-like lookup.
  /// </summary>
  /// <typeparam name="TKey">The key type.</typeparam>
  /// <typeparam name="Token">The token type. The token is invalidated when the compared tokens are not equal</typeparam>
  /// <typeparam name="TValue">The value type.</typeparam>
  /// <typeparam name="TParam">A parameter type used to generate updated values when the cached value is invalidated</typeparam>
  public class Cache<TKey, Token, TValue, TParam> where Token : struct
  {
    private readonly Dictionary<TKey, CacheItem<Token, TValue, TParam>> cache;

    /// <summary>Creates a cache</summary>
    public Cache()
    {
      cache = new Dictionary<TKey, CacheItem<Token, TValue, TParam>>();
    }

    /// <summary>Creates a cache with an initial capacity</summary>
    /// <param name="capacity"></param>
    public Cache(int capacity)
    {
      cache = new Dictionary<TKey, CacheItem<Token, TValue, TParam>>(capacity);
    }

    /// <summary>Retrieves the number of elements in the cache</summary>
    public int Count { get { return cache.Count; } }

    /// <summary>
    /// Defines a cache key if the key does not exist, otherwise flushes the key.
    /// </summary>
    /// <param name="key">The key containing the cached token and value</param>
    /// <param name="token">The new token to be compared with the cached token</param>
    public void Define(TKey key, Token token)
    {
      CacheItem<Token, TValue, TParam> ret = new CacheItem<Token, TValue, TParam>(token);
      if (cache.ContainsKey(key))
        cache[key] = ret;
      else
        cache.Add(key, ret);
    }

    /// <summary>
    /// Retrieves the value based on the cached key.
    /// </summary>
    /// <param name="key">The key containing the cached token and value</param>
    /// <param name="token">The new token to be compared with the cached token</param>
    /// <param name="func">The function used to generate the updated value</param>
    /// <param name="p">The parameter value used to generate the updated value</param>
    /// <param name="cmp">The token comparer</param>
    /// <returns>If the tokens match, return the cached value, otherwise update this value with func(p) and returns the new value</returns>
    /// <exception cref="InvalidOperationException">Attempted to get an non-existent key from a cache.</exception>
    public TValue Get(TKey key, Token token, Func<TParam, TValue> func, TParam p, IEqualityComparer<Token> cmp)
    {
      CacheItem<Token, TValue, TParam> item;
      if (!cache.TryGetValue(key, out item))
        throw new InvalidOperationException("Attempted to get an non-existent key '{0}' from a cache.".F(key));

      return item.Get(token, func, p, cmp);
    }

    /// <summary>
    /// Defines a cache key if the key does not exist, then retrieves the value.
    /// </summary>
    /// <param name="key">The key containing the cached token and value</param>
    /// <param name="token">The new token to be compared with the cached token</param>
    /// <param name="func">The function used to generate the updated value</param>
    /// <param name="p">The parameter value used to generate the updated value</param>
    /// <param name="cmp">The token comparer</param>
    /// <returns>If the tokens match, return the cached value, otherwise update this value with func(p) and returns the new value</returns>
    public TValue GetOrDefine(TKey key, Token token, Func<TParam, TValue> func, TParam p, IEqualityComparer<Token> cmp)
    {
      CacheItem<Token, TValue, TParam> item;
      if (!cache.TryGetValue(key, out item))
      {
        cache.Add(key, new CacheItem<Token, TValue, TParam>(default(Token)));
        item = cache[key];
      }
      return item.Get(token, func, p, cmp);
    }

    /// <summary>Clears the cache of entries depending on a conditional function.</summary>
    /// <param name="func">The function that determines whether the entry should be cleared. Use null to clear the entire cache.</param>
    public void Clear(Func<Token, bool> func = null)
    {
      if (func == null)
        cache.Clear();
      else
        foreach (TKey k in new List<TKey>(cache.Keys))
          if (func(cache[k].ExpiryToken))
            cache.Remove(k);
    }

    private class CacheItem<E, T, TP> where E : struct
    {
      internal E ExpiryToken;
      private T val;

      public CacheItem(E token)
      {
        ExpiryToken = token;
        val = default(T);
      }

      public T Get(E token, Func<TP, T> func, TP p)
      {
        if (!EqualityComparer<E>.Default.Equals(ExpiryToken, token))
        {
          val = func(p);
          ExpiryToken = token;
        }
        return val;
      }

      public T Get(E token, Func<TP, T> func, TP p, IEqualityComparer<E> cmp)
      {
        if (!cmp.Equals(ExpiryToken, token))
        {
          val = func(p);
          ExpiryToken = token;
        }
        return val;
      }
    }
  }
}

