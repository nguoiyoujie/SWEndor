using System;
using System.Collections.Generic;

namespace SWEndor.Primitives
{
  public class Cache<TKey, Token, TValue> where Token : struct
  {
    private readonly Dictionary<TKey, CacheItem<Token, TValue>> cache = new Dictionary<TKey, CacheItem<Token, TValue>>();

    public void Define(TKey key, Token token, Func<TValue> func)
    {
      CacheItem<Token, TValue> ret = new CacheItem<Token, TValue>(token, func);
      if (cache.ContainsKey(key))
        cache[key] = ret;
      else
        cache.Add(key, ret);
    }

    public TValue Get(TKey key, Token token)
    {
      CacheItem<Token, TValue> item;
      if (!cache.TryGetValue(key, out item))
        throw new InvalidOperationException("Attempted to get an non-existent key '{0}' from a cache.".F(key));

      return item.Get(token);
    }

    public TValue GetOrDefine(TKey key, Token token, Func<TValue> func)
    {
      CacheItem<Token, TValue> item;
      if (!cache.TryGetValue(key, out item))
      {
        cache.Add(key, new CacheItem<Token, TValue>(token, func));
        item = cache[key];
      }
      return item.Get(token);
    }

    public void Clear(Func<Token, bool> func = null)
    {
      if (func == null)
      {
        cache.Clear();
        return;
      }

      foreach (TKey k in cache.Keys)
      {
        if (func(cache[k].ExpiryToken))
          cache.Remove(k);
      }
    }

    private struct CacheItem<E, T> where E : struct
    {
      internal E ExpiryToken;
      private Func<T> fn;
      private T val;

      public CacheItem(E token, Func<T> func)
      {
        fn = func;
        ExpiryToken = token;
        val = fn();
      }

      public T Get(E token)
      {
        if (!EqualityComparer<E>.Default.Equals(ExpiryToken, token))
        {
          val = fn();
          ExpiryToken = token;
        }
        return val;
      }
    }
  }
}

