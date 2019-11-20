using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class ExecutorStaticATI : Groups.StaticScene
  {
    internal ExecutorStaticATI(Factory owner) : base(owner, "EXECSTS", "Executor Static")
    {
      MeshData = new MeshData(Engine, Name, @"executor\executor_static.x");
    }
  }
}

