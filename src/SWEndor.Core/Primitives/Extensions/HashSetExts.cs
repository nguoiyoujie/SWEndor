using SWEndor.Models;
using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.Extensions
{
  public static class HashSetExts
  {
    static Predicate<IActorDisposable> rem = (a) => (a == null || a.DisposingOrDisposed);
    public static void RemoveDisposed<T>(this HashSet<T> list)
      where T : IActorDisposable
    {
      //foreach (ActorInfo a in new List<ActorInfo>(list))
      //  if (a != null && a.DisposingOrDisposed)
      //    list.Remove(a);

      list.RemoveWhere((Predicate<T>)(object)rem);
    }
  }
}
