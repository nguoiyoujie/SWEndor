using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.Traits
{
  public enum TraitCollectionState { INIT, ACTIVE, DISPOSED }

  public class TraitCollection
  {
    readonly Dictionary<Type, List<ITrait>> traits = new Dictionary<Type, List<ITrait>>();
    readonly ITraitOwner owner;
    internal TraitCollectionState State;

    public TraitCollection(ITraitOwner owner)
    {
      this.owner = owner;
    }

    public IEnumerable<T> TraitsImplementing<T>() where T : ITrait
    {
      Type type = typeof(T);
      //CheckDisposed(type);
      if (traits.ContainsKey(type))
        foreach (ITrait t in traits[type])
          yield return (T)t;
    }

    public T Add<T>(T val)
      where T : ITrait
    {
      var t = val.GetType();

      foreach (Type i in t.GetInterfaces())
        if (typeof(ITrait).IsAssignableFrom(i))
          InnerAdd(i, val);
      foreach (Type tt in t.BaseTypes())
        if (typeof(ITrait).IsAssignableFrom(tt))
          InnerAdd(tt, val);

      return val;
    }

    public void Clear()
    {
      traits.Clear();
    }

    public T Get<T>()
      where T : ITrait
    {
      T ret = default(T);
      if (!TryGetOne(out ret))
          throw new InvalidOperationException("TraitOwner does not have trait of type `{0}`".F(typeof(T)));
      return ret;
    }

    public bool TryGet<T>(out T trait)
      where T : ITrait
    {
      return TryGetOne(out trait);
    }

    public T GetOrDefault<T>()
      where T : ITrait
    {
      T ret = default(T);
      TryGetOne(out ret);        
      return ret;
    }

    public T GetOrCreate<T>()
      where T : ITrait, new()
    {
      T ret = default(T);
      if (!TryGetOne(out ret))
        Add(new T());
      return ret;
    }

    private bool TryGetOne<T>(out T ret)
      where T : ITrait
    {
      ret = default(T);
      //CheckDisposed(typeof(T));
      Type type = typeof(T);
      if (!traits.ContainsKey(type))
        return false;

      if (traits[type] == null || (traits[type].Count == 0))
        return false;

      if (traits[type].Count > 1)
        throw new InvalidOperationException("TraitOwner has multiple traits of type `{0}`".F(typeof(T)));//owner.Name, typeof(T)));

      ret = (T)(traits[type][0]);
      return true;
    }

    private void InnerAdd<T>(Type type, T val)
      where T : ITrait
    {
      if (!traits.ContainsKey(type))
        traits.Add(type, new List<ITrait>());

      traits[type].Add(val);
    }

    private void CheckDisposed(Type type)
    {
      if (State == TraitCollectionState.DISPOSED || (State == TraitCollectionState.ACTIVE && owner.Disposed))
        throw new InvalidOperationException("Attempted to get trait '{0}' from disposed object ({1})".F(type.Name, owner));
    }
  }

  public class TraitDictionary
  {
    static readonly Func<Pair<Type,Type>, ITraitContainer> CreateTraitContainer = p => (ITraitContainer)typeof(TraitContainer<,>).MakeGenericType(p.u, p.v).GetConstructor(Type.EmptyTypes).Invoke(null);

    readonly Dictionary<Pair<Type, Type>, ITraitContainer> traits = new Dictionary<Pair<Type, Type>, ITraitContainer>();

    ITraitContainer InnerGet(Pair<Type, Type> p)
    {
      return traits.GetOrAdd(p, CreateTraitContainer);
    }

    TraitContainer<A,T> InnerGet<A,T>()
      where A : ITraitOwner
      where T : ITrait
    {
      return (TraitContainer<A, T>)InnerGet(new Pair<Type, Type>(typeof(A), typeof(T)));
    }

    public void AddTrait<T>(ITraitOwner owner, T val)
      where T : ITrait
    {
      var t = val.GetType();

      foreach (Type i in t.GetInterfaces())
        if (typeof(ITrait).IsAssignableFrom(i))
          InnerAdd(owner, i, val);
      foreach (Type tt in t.BaseTypes())
        if (typeof(ITrait).IsAssignableFrom(tt))
          InnerAdd(owner, tt, val);
    }

    void InnerAdd<T>(ITraitOwner owner, Type t, T val)
       where T : ITrait
    {
      InnerGet(new Pair<Type, Type>(owner.GetType(), t)).Add(owner, val);
    }

    static void CheckDisposed<A>(A owner)
      where A : ITraitOwner
    {
      if (owner.Disposed)
        throw new InvalidOperationException("Attempted to get trait from disposed object ({0})".F(owner));
    }

    public T Get<A,T>(A owner)
      where A : ITraitOwner
      where T : ITrait
    {
      CheckDisposed(owner);
      return InnerGet<A,T>().Get(owner.ID);
    }

    public T GetOrDefault<A,T>(A owner)
      where A : ITraitOwner
      where T : ITrait
    {
      CheckDisposed(owner);
      return InnerGet<A,T>().GetOrDefault(owner.ID);
    }

    public IEnumerable<T> WithInterface<A,T>(A owner)
      where A : ITraitOwner
      where T : ITrait
    {
      CheckDisposed(owner);
      return InnerGet<A,T>().GetMultiple(owner.ID);
    }

    public IEnumerable<TraitPair<A,T>> ActorsWithTrait<A,T>()
      where A : ITraitOwner
      where T : ITrait
    {
      return InnerGet<A,T>().All();
    }

    public IEnumerable<A> HavingTrait<A,T>()
      where A : ITraitOwner
      where T : ITrait
    {
      return InnerGet<A,T>().Actors();
    }

    public IEnumerable<A> HavingTrait<A,T>(Func<T, bool> predicate)
      where A : ITraitOwner
      where T : ITrait
    {
      return InnerGet<A,T>().Actors(predicate);
    }

    public void RemoveActor<A>(A a)
       where A : ITraitOwner
    {
      foreach (var t in traits)
        t.Value.RemoveActor(a.ID);
    }

    interface ITraitContainer
    {
      void Add(ITraitOwner actor, object trait);
      void RemoveActor(int id);

      int Queries { get; }
    }

    class TraitContainer<A,T> : ITraitContainer
        where A : ITraitOwner
        where T : ITrait
    {
      readonly List<A> actors = new List<A>();
      readonly List<T> traits = new List<T>();

      public int Queries { get; private set; }

      public void Add(ITraitOwner actor, object trait)
      {
        if (actor == null)
        { }

        int insertIndex = actors.BinarySearchMany(actor.ID + 1);
        actors.Insert(insertIndex, (A)actor);
        traits.Insert(insertIndex, (T)trait);
      }

      public T Get(int id)
      {
        var result = GetOrDefault(id);
        if (result == null)
          throw new InvalidOperationException("TraitOwner does not have trait of type `{0}`".F(typeof(T)));
        return result;
      }

      public T GetOrDefault(int id)
      {
        ++Queries;
        var index = actors.BinarySearchMany(id);
        if (index >= actors.Count || actors[index].ID != id)
          return default(T);
        else if (index + 1 < actors.Count && actors[index + 1].ID == id)
          throw new InvalidOperationException("TraitOwner {0} has multiple traits of type `{1}`".F(actors[index].Name, typeof(T)));
        else return traits[index];
      }

      public IEnumerable<T> GetMultiple(int actor)
      {
        ++Queries;
        return new MultipleEnumerable(this, actor);
      }

      class MultipleEnumerable : IEnumerable<T>
      {
        readonly TraitContainer<A,T> container;
        readonly int actor;
        public MultipleEnumerable(TraitContainer<A,T> container, int actor) { this.container = container; this.actor = actor; }
        public IEnumerator<T> GetEnumerator() { return new MultipleEnumerator(container, actor); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
      }

      class MultipleEnumerator : IEnumerator<T>
      {
        readonly List<A> actors;
        readonly List<T> traits;
        readonly int id;
        int index;
        public MultipleEnumerator(TraitContainer<A,T> container, int id)
        {
          actors = container.actors;
          traits = container.traits;
          this.id = id;
          Reset();
        }

        public void Reset() { index = actors.BinarySearchMany(id) - 1; }
        public bool MoveNext() {
          return ++index < actors.Count && actors[index].ID == id;
        }
        public T Current { get { return traits[index]; } }
        object System.Collections.IEnumerator.Current { get { return Current; } }
        public void Dispose() { }
      }

      public IEnumerable<TraitPair<A,T>> All()
      {
        // PERF: Custom enumerator for efficiency - using `yield` is slower.
        ++Queries;
        return new AllEnumerable(this);
      }

      public IEnumerable<A> Actors()
      {
        ++Queries;
        A last = default(A);
        for (var i = 0; i < actors.Count; i++)
        {
          if (actors[i].ID == last.ID)
            continue;
          yield return actors[i];
          last = actors[i];
        }
      }

      public IEnumerable<A> Actors(Func<T, bool> predicate)
      {
        ++Queries;
        A last = default(A);

        for (var i = 0; i < actors.Count; i++)
        {
          if (actors[i].ID == last.ID || !predicate(traits[i]))
            continue;
          yield return actors[i];
          last = actors[i];
        }
      }

      class AllEnumerable : IEnumerable<TraitPair<A,T>>
      {
        readonly TraitContainer<A,T> container;
        public AllEnumerable(TraitContainer<A,T> container) { this.container = container; }
        public IEnumerator<TraitPair<A,T>> GetEnumerator() { return new AllEnumerator(container); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
      }

      class AllEnumerator : IEnumerator<TraitPair<A,T>>
      {
        readonly List<A> actors;
        readonly List<T> traits;
        int index;
        public AllEnumerator(TraitContainer<A,T> container)
        {
          actors = container.actors;
          traits = container.traits;
          Reset();
        }

        public void Reset() { index = -1; }
        public bool MoveNext() { return ++index < actors.Count; }
        public TraitPair<A,T> Current { get { return new TraitPair<A,T>(actors[index], traits[index]); } }
        object System.Collections.IEnumerator.Current { get { return Current; } }
        public void Dispose() { }
      }

      public void RemoveActor(int id)
      {
        var startIndex = actors.BinarySearchMany(id);
        if (startIndex >= actors.Count || actors[startIndex].ID != id)
          return;
        var endIndex = startIndex + 1;
        while (endIndex < actors.Count && actors[endIndex].ID == id)
          endIndex++;
        var count = endIndex - startIndex;
        actors.RemoveRange(startIndex, count);
        traits.RemoveRange(startIndex, count);
      }
    }
  }
}
