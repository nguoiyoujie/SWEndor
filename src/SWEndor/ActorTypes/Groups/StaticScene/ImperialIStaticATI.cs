using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class ImperialIStaticATI : Groups.StaticScene
  {
    internal ImperialIStaticATI(Factory owner) : base(owner, "ISD_STS", "Imperial-I Static")
    {
      MeshData = new MeshData(Engine, Name, @"stardestroyer\star_destroyer_static.x");
    }
  }
}

