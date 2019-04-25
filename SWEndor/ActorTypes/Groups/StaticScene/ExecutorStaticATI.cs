using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorStaticATI : Groups.StaticScene
  {
    internal ExecutorStaticATI(Factory owner) : base(owner, "Executor Static")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_static.x");
    }
  }
}

