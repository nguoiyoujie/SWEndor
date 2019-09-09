using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorStaticATI : Groups.StaticScene
  {
    internal ExecutorStaticATI(Factory owner) : base(owner, "Executor Static")
    {
      MeshData = new MeshData(Name, @"executor\executor_static.x");
    }
  }
}

