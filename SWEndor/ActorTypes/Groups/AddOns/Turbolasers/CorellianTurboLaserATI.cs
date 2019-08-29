using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class CorellianTurboLaserATI : Groups.Turbolasers
  {
    internal CorellianTurboLaserATI(Factory owner) : base(owner, "Corellian Turbolaser Tower")
    {
      MaxStrength = 105;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\corellian_turbolaser.x");

      Loadouts = new string[] { "CRLN_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

