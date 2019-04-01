using System.IO;

namespace SWEndor.ActorTypes
{
  public class ExecutorStaticATI : StaticSceneGroup
  {
    private static ExecutorStaticATI _instance;
    public static ExecutorStaticATI Instance()
    {
      if (_instance == null) { _instance = new ExecutorStaticATI(); }
      return _instance;
    }

    private ExecutorStaticATI() : base("Executor Static")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_static.x");
    }
  }
}

