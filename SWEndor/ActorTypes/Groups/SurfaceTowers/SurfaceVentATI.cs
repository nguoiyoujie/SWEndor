using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SurfaceVentATI : Groups.SurfaceTower
  {
    internal SurfaceVentATI(Factory owner) : base(owner, "Thermal Exhaust Port")
    {
      MaxStrength = 12000;
      ImpactDamage = 120;

      RadarSize = 2.5f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface_vent.x");
    }

    public override void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(owner, hitby, impact, normal);

      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo is TorpedoATI) //hard code?
        owner.SetState_Dead();
    }
  }
}

