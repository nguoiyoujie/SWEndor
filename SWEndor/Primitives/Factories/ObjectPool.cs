using System;
using System.Threading;

namespace SWEndor.Primitives.Factories
{
  public interface IPoolable
  {
    bool Disposed { get; set; }
    void Reset();
    IPoolable Next { get; set; }
    IPoolable Prev { get; set; }
    int ID { get; set; }
  }

  public class ObjectPool<T> where T : IPoolable, new()
  {
    public ObjectPool(int capacity)
    {
      Capacity = capacity;
      list = new T[Capacity];
    }

    public readonly int Capacity;

    #pragma warning disable CS0649
    private T deadT;
    #pragma warning restore CS0649
    
    private T[] list;
    private int count = 0;
    private int counter = 0;
    private int emptycounter = 0;
    private Mutex mu_counter = new Mutex();
    private T First { get; set; }
    private T Last { get; set; }

    public T ActivateNext()
    {
      try
      {
        mu_counter.WaitOne();
        counter = emptycounter;
        while (counter < list.Length && (list[counter] != null && list[counter].Disposed))
          counter++;

        if (counter >= list.Length)
          throw new Exception(string.Format("The list of {0} has exceeded capacity!", typeof(T).Name));

        int i = counter;
        emptycounter = i + 1;

        if (list[i] == null)
        {
          list[i] = new T();
        }
        else
        {
          list[i].ID += Capacity;
        }

        if (First.Equals(deadT))
          First = list[i];
        else
        {
          Last.Next = list[i];
          list[i].Prev = Last;
        }
        Last = list[i];
        count++;

        return list[i];
      }
      finally
      {
        mu_counter.ReleaseMutex();
      }
    }

    public T Get(int index)
    {
      if (index < 0)
        throw new ArgumentException("ID cannot be negative!");

      return list[index % Capacity];
    }

    public void Dispose(int id)
    {
      if (id < 0)
        throw new ArgumentException("ID cannot be negative!");

      int x = id % Capacity;

      if (!list[x].Disposed)
      {
        try
        {
          mu_counter.WaitOne();

          count--;

          if (First.Equals(list[x]) && Last.Equals(list[x]))
          {
            First = deadT;
            Last = deadT;
          }
          else if (First.Equals(list[x]))
          {
            First = (T)list[x].Next;
          }
          else if (Last.Equals(list[x]))
          {
            Last = (T)list[x].Prev;
          }
          else
          {
            list[x].Prev.Next = list[x].Next;
            list[x].Next.Prev = list[x].Prev;
          }
          list[x].Disposed = true;

          if (x < emptycounter)
            emptycounter = x;
        }
        finally
        {
          mu_counter.ReleaseMutex();
        }
      }
    }

    public void Reset()
    {
      for (int x = 0; x < list.Length; x++)
      {
        list[x].Disposed = true;
        list[x] = deadT;
      }
      First = deadT;
      Last = deadT;
    }

    public void Do(Engine engine, int id, Action<Engine, T> action)
    {
      action.Invoke(engine, list[id % Capacity]);
    }

    public void DoEach(Engine engine, Action<Engine, T> action)
    {
      IPoolable unit = First;
      for (int i = 0; i < count && !unit.Equals(deadT); i++)
      {
        action.Invoke(engine, (T)unit);
        unit = unit.Next;
      }
    }

    public void DoIf(Engine engine, int id, Func<Engine, T, bool> condition, Action<Engine, T> action)
    {
      int index = id % Capacity;
      if (condition(engine, list[index]))
        action.Invoke(engine, list[index]);
    }

    public void DoEachIf(Engine engine, int id, Func<Engine, T, bool> condition, Action<Engine, T> action)
    {
      IPoolable unit = First;
      for (int i = 0; i < count && !unit.Equals(deadT); i++)
      {
        if (condition(engine, (T)unit))
          action.Invoke(engine, (T)unit);
        unit = unit.Next;
      }
    }
  }
}
