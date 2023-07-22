using Primrose.Primitives.Factories;

namespace SWEndor.Game.AI.Squads
{
  public partial class Squadron
  {
    static Squadron()
    {
      ObjectPool<Squadron>.CreateStaticPool(() => new Squadron(), (p) => p.Reset());
    }

    public class Factory : Registry<int, Squadron>
    {
      private int counter = 0;
      private object creationLock = new object();

      public Squadron Create()
      {
        Squadron squad = ObjectPool<Squadron>.GetStaticPool().GetNew();

        lock (creationLock)
        {
          int id = counter++;
          squad.ID = id;
          Add(id, squad);
        }
        return squad;
      }

      public Squadron Create(string name)
      {
        Squadron squad = Create();
        squad.Name = name;
        return squad;
      }

      public void Return(Squadron s)
      {
        lock (creationLock)
          Remove(s.ID);

        ObjectPool<Squadron>.GetStaticPool().Return(s);
      }

      public Squadron GetByName(string name)
      {
        foreach (Squadron s in GetValues())
          if (s.Name == name)
            return s;
        return Neutral;
      }

      public void Reset()
      {
        counter = 0;
      }
    }
  }
}