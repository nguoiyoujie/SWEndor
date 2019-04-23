using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ImperialIStaticATI : Groups.StaticScene
  {
    private static ImperialIStaticATI _instance;
    public static ImperialIStaticATI Instance()
    {
      if (_instance == null) { _instance = new ImperialIStaticATI(); }
      return _instance;
    }

    private ImperialIStaticATI() : base("Imperial-I Static")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_static.x");
    }
  }
}

