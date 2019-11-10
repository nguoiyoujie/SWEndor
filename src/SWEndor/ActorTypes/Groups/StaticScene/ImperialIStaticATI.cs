using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class ImperialIStaticATI : Groups.StaticScene
  {
    internal ImperialIStaticATI(Factory owner) : base(owner, "IMPLSTS", "Imperial-I Static")
    {
      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_static.x");
    }
  }
}

