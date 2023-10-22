using MTV3D65;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Models;

namespace SWEndor.Game.ActorTypes.Instances
{
  internal class DeathStarATI : Groups.StaticScene
  {
    internal DeathStarATI(Factory owner) : base(owner, "DSTAR", "DeathStar")
    {
      ExplodeSystemData.Explodes = new ExplodeData[]
      {
        new ExplodeData(new string[] { "EXPL02" }, default, 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeData(new string[] { "EXPW02" }, default, 1, 1, ExplodeTrigger.ON_DEATH),
      };

      MeshData = MeshDataDecorator.CreateAlphaTexturedWall(Engine, Name, 20000, "deathstar/deathstar.bmp", "deathstar/deathstaralpha.bmp", CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
      TimedLifeData = new TimedLifeData(false, 5);
    }
  }
}


