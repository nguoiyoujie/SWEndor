using System.Collections.Concurrent;

namespace Primrose.Primitives.Pipelines
{
  /// <summary>
  /// Maintains and executes objects in an queue. Useful for FIFO procedures like UI modifications.
  /// </summary>
  /// <typeparam name="T">A piped object</typeparam>
  public class Pipeline<T> where T : IPipedObject
  {
    private ConcurrentQueue<T> pipe = new ConcurrentQueue<T>();

    /// <summary>The maximum number of piped objects to be executed per call to Run()</summary>
    public int MaxExecutionsPerRun = 1;

    /// <summary>
    /// Creates a pipeline
    /// </summary>
    /// <param name="maxExecutionsPerRun">The maximum number of piped objects to be executed per call to Run()</param>
    public Pipeline(int maxExecutionsPerRun = 1) { MaxExecutionsPerRun = maxExecutionsPerRun; }

    /// <summary>Queues an piped object into the pipeline</summary>
    /// <param name="item"></param>
    public void Queue(T item) { pipe.Enqueue(item); }

    /// <summary>Runs the execution of queued objects, up to a limit defined by MaxExecutionsPerRun</summary>
    public void Run()
    {
      int x = MaxExecutionsPerRun;
      T pobj = default(T);
      while (x-- > 0 && pipe.TryDequeue(out pobj))
      {
        pobj.Execute();
      }
    }

    /// <summary>Clears the queue without execution</summary>
    public void Clear()
    {
      T pobj = default(T);
      while (pipe.TryDequeue(out pobj)) { }
    }
  }

  /// <summary>
  /// Defines a piped object to be processed in a pipeline
  /// </summary>
  public interface IPipedObject
  {
    /// <summary>Provides execution entry point from the pipeline</summary>
    void Execute();
  }
}