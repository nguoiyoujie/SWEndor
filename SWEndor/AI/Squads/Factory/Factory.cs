using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEndor.AI.Squads
{
  public partial class Squadron
  {
    public class Factory : Primitives.Factories.Registry<int, Squadron>
    {
      private int counter = 0;
      private object creationLock = new object();

      public Squadron Create()
      {
        Squadron squad = null;

        lock (creationLock)
        {
          int id = counter++;
          squad = new Squadron();
          squad.ID = id;
        }

        return squad;
      }
    }
  }
}