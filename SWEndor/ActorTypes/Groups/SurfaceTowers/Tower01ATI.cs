using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower01ATI : Groups.SurfaceTower
  {
    internal Tower01ATI(Factory owner) : base(owner, "Deflector Tower")
    {
      MaxStrength = 100;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_01.x");
      AddOns = new AddOnInfo[] { new AddOnInfo("Advanced Turbolaser Turret", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }
  }
}

