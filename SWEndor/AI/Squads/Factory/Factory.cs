using SWEndor.Primitives.Factories;

namespace SWEndor.AI.Squads
{
  public partial class Squadron
  {
    public class Factory : Primitives.Factories.Registry<int, Squadron>
    {
      private int counter = 0;
      private object creationLock = new object();
      private ObjectPool<Squadron> pool;

      public Factory()
      {
        pool = new ObjectPool<Squadron>(() => new Squadron(), (p) => p.Reset());
      }

      public Squadron Create()
      {
        Squadron squad = pool.GetNew();

        lock (creationLock)
        {
          int id = counter++;
          squad.ID = id;
        }
        return squad;
      }

      public void Return(Squadron s)
      {
        lock (creationLock)
          Remove(s.ID);

        pool.Return(s);
      }

      public void Reset()
      {
        counter = 0;
      }
    }
  }
}