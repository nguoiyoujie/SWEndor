using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class mc90TurbolaserATI : Groups.Turbolasers
  {
    internal mc90TurbolaserATI(Factory owner) : base(owner, "MC90 Turbolaser Tower")
    {
      MaxStrength = 200;
      ImpactDamage = 16;
      MaxTurnRate = 50f;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\mc90_turbolaser.x");

      Loadouts = new string[] { "MC90_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

