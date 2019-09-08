using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBMissilePodATI : Groups.Turbolasers
  {
    internal NebulonBMissilePodATI(Factory owner) : base(owner, "Nebulon B Missile Pod")
    {
      MaxStrength = 140; //32
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\nebulonb_missilepod.x");

      Loadouts = new string[] { "NEBL_MISL" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

