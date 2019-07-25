using SWEndor.Primitives.Factories;
using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.Traits
{
  public class TraitPoolCollection
  {
    readonly Dictionary<Type, ITraitPool> pools = new Dictionary<Type, ITraitPool>();

    public T GetTrait<T>() where T : ITrait
    {
      Type t = typeof(T);
      if (!pools.ContainsKey(t))
        pools.Add(t, (ITraitPool)typeof(TraitPool<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null));

      return (T)pools[t].GetTrait();
    }

    public void ReturnTrait<T>(T trait) where T : ITrait
    {
      Type t = trait.GetType();
      if (!pools.ContainsKey(t))
        return; // don't do anything if the GetTrait did not generate the pool.

        //pools.Add(t, (ITraitPool)typeof(TraitPool<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null));
      pools[t].ReturnTrait(trait);
    }
  }

  public interface ITraitPool
  {
    object GetTrait();
    void ReturnTrait(object item);
  }

  public class TraitPool<T> : ObjectPool<T>, ITraitPool where T : ITrait, new()
  {
    public TraitPool() : base(() => { return new T(); }, (t) => { }) { }
    public TraitPool(Func<T> createFn, Action<T> resetFn) : base(createFn, resetFn) { }

    public object GetTrait() { return GetNew(); }
    public void ReturnTrait(object item) { Return((T)item); }
  }
}
