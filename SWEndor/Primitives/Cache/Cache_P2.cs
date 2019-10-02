using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace SWEndor.Primitives
{
  public class Cache<TKey, Token, TValue, TParam1, TParam2> where Token : struct
  {
    private readonly Dictionary<TKey, CacheItem<Token, TValue, TParam1, TParam2>> cache; //= new Dictionary<TKey, CacheItem<Token, TValue, TParam1, TParam2>>();

    public Cache()
    {
      cache = new Dictionary<TKey, CacheItem<Token, TValue, TParam1, TParam2>>();
    }

    public Cache(int capacity)
    {
      cache = new Dictionary<TKey, CacheItem<Token, TValue, TParam1, TParam2>>(capacity);
    }

    public void Define(TKey key, Token token, Func<TParam1, TParam2, TValue> func)
    {
      CacheItem<Token, TValue, TParam1, TParam2> ret = new CacheItem<Token, TValue, TParam1, TParam2>(token);
      if (cache.ContainsKey(key))
        cache[key] = ret;
      else
        cache.Add(key, ret);
    }

    public TValue Get(TKey key, Token token, Func<TParam1, TParam2, TValue> func, TParam1 p1, TParam2 p2, IEqualityComparer<Token> cmp)
    {
      CacheItem<Token, TValue, TParam1, TParam2> item;
      if (!cache.TryGetValue(key, out item))
        throw new InvalidOperationException("Attempted to get an non-existent key '{0}' from a cache.".F(key));

      return item.Get(token, func, p1, p2, cmp);
    }

    public TValue GetOrDefine(TKey key, Token token, Func<TParam1, TParam2, TValue> func, TParam1 p1, TParam2 p2, IEqualityComparer<Token> cmp)
    {
      CacheItem<Token, TValue, TParam1, TParam2> item;
      if (!cache.TryGetValue(key, out item))
      {
        cache.Add(key, new CacheItem<Token, TValue, TParam1, TParam2>(default(Token)));
        item = cache[key];
      }
      return item.Get(token, func, p1, p2, cmp);
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

    private struct CacheItem<E, T, TP1, TP2> where E : struct
    {
      internal E ExpiryToken;
      private T val;

      public CacheItem(E token)
      {
        ExpiryToken = token;
        val = default(T);
      }

      public T Get(E token, Func<TP1, TP2, T> func, TP1 p1, TP2 p2)
      {
        if (!EqualityComparer<E>.Default.Equals(ExpiryToken, token))
        {
          val = func(p1, p2);
          ExpiryToken = token;
        }
        return val;
      }

      public T Get(E token, Func<TP1, TP2, T> func, TP1 p1, TP2 p2, IEqualityComparer<E> cmp)
      {
        if (!cmp.Equals(ExpiryToken, token))
        {
          val = func(p1, p2);
          ExpiryToken = token;
        }
        return val;
      }
    }
  }
}

