using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBTurboLaserATI : Groups.Turbolasers
  {
    internal NebulonBTurboLaserATI(Factory owner) : base(owner, "Nebulon B Turbolaser Tower")
    {
      MaxStrength = 135;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\nebulonb_turbolaser.x");

      Loadouts = new string[] { "NEBL_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

