using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarATI : Groups.StaticScene
  {
    internal DeathStarATI(Factory owner) : base(owner, "DeathStar")
    {
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

      MeshData = MeshDataDecorator.CreateAlphaTexturedWall(Name, 20000, "deathstar/deathstar.bmp", "deathstar/deathstaralpha.bmp");
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      ainfo.DyingTimerSet(5, true);
      CombatSystem.Deactivate(Engine, ainfo);
    }
  }
}


