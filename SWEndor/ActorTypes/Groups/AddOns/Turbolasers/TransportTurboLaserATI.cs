using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TransportTurboLaserATI : Groups.Turbolasers
  {
    internal TransportTurboLaserATI(Factory owner) : base(owner, "Transport Turbolaser Tower")
    {
      MaxStrength = 80;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\transport_turbolaser.x");

      Loadouts = new string[] { "TRNS_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

