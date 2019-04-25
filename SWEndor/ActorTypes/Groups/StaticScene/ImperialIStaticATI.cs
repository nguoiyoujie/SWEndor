using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ImperialIStaticATI : Groups.StaticScene
  {
    internal ImperialIStaticATI(Factory owner) : base(owner, "Imperial-I Static")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_static.x");
    }
  }
}

