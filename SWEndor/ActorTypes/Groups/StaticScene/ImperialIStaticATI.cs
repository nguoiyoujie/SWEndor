using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ImperialIStaticATI : Groups.StaticScene
  {
    internal ImperialIStaticATI(Factory owner) : base(owner, "Imperial-I Static")
    {
      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_static.x");
    }
  }
}

