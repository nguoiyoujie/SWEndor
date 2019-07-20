using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower03ATI : Groups.SurfaceTower
  {
    internal Tower03ATI(Factory owner) : base(owner, "Radar Tower")
    {
      MaxStrength = 75;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      RadarSize = 2.5f;

      AlwaysShowInRadar = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_03.x");
    }
  }
}

