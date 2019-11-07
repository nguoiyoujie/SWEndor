using System.Collections.Concurrent;

namespace SWEndor.Primitives.Pipelines
{
  /// <summary>
  /// Maintains and executes an ordered queue. Useful for linear procedures like UI modifications.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Pipeline<T> where T : IPipedObject
  {
    private ConcurrentQueue<T> pipe = new ConcurrentQueue<T>();
    public int MaxExecutionsPerRun = 1;

    public Pipeline(int maxExecutionsPerRun = 1) { MaxExecutionsPerRun = maxExecutionsPerRun; }

    public void Queue(T item) { pipe.Enqueue(item); }

    public void Run()
    {
      int x = MaxExecutionsPerRun;
      T pobj = default(T);
      while (x-- > 0 && pipe.TryDequeue(out pobj))
      {
        pobj.Execute();
      }
    }

    public void Clear()
    {
      T pobj = default(T);
      while (pipe.TryDequeue(out pobj)) { }
    }
  }

  public interface IPipedObject
  {
    void Execute();
  }
}