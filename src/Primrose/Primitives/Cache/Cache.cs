using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace Primrose.Primitives
{
  public class Cache<TKey, Token, TValue, TParam> where Token : struct
  {
    private readonly Dictionary<TKey, CacheItem<Token, TValue, TParam>> cache;

    public Cache()
    {
      cache = new Dictionary<TKey, CacheItem<Token, TValue, TParam>>();
    }

    public Cache(int capacity)
    {
      cache = new Dictionary<TKey, CacheItem<Token, TValue, TParam>>(capacity);
    }

    public int Count { get { return cache.Count; } }

    public void Define(TKey key, Token token, Func<TParam, TValue> func)
    {
      CacheItem<Token, TValue, TParam> ret = new CacheItem<Token, TValue, TParam>(token);
      if (cache.ContainsKey(key))
        cache[key] = ret;
      else
        cache.Add(key, ret);
    }

    public TValue Get(TKey key, Token token, Func<TParam, TValue> func, TParam p, IEqualityComparer<Token> cmp)
    {
      CacheItem<Token, TValue, TParam> item;
      if (!cache.TryGetValue(key, out item))
        throw new InvalidOperationException("Attempted to get an non-existent key '{0}' from a cache.".F(key));

      return item.Get(token, func, p, cmp);
    }

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

    public void Clear(Func<Token, bool> func = null)
    {
      if (func == null)
      {
        cache.Clear();
        return;
      }

      foreach (TKey k in new List<TKey>(cache.Keys))
      {
        if (func(cache[k].ExpiryToken))
          cache.Remove(k);
      }
    }

    private struct CacheItem<E, T, TP> where E : struct
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

