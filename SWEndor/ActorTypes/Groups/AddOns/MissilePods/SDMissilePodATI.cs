using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDMissilePodATI : Groups.Turbolasers
  {
    internal SDMissilePodATI(Factory owner) : base(owner, "Star Destroyer Missile Pod")
    {
      MaxStrength = 24; //32
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_missilepod.x");

      Loadouts = new string[] { "IMPL_MISL" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

