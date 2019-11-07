using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarATI : Groups.StaticScene
  {
    internal DeathStarATI(Factory owner) : base(owner, "DSTAR", "DeathStar")
    {
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

      MeshData = MeshDataDecorator.CreateAlphaTexturedWall(Name, 20000, "deathstar/deathstar.bmp", "deathstar/deathstaralpha.bmp", CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
      TimedLifeData = new TimedLifeData(false, 5);
    }
  }
}


