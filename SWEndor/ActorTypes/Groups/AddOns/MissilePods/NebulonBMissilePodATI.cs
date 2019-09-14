using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBMissilePodATI : Groups.Turbolasers
  {
    internal NebulonBMissilePodATI(Factory owner) : base(owner, "Nebulon B Missile Pod")
    {
      MaxStrength = 140; //32
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\nebulonb_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "NEBL_MISL" };
    }
  }
}

