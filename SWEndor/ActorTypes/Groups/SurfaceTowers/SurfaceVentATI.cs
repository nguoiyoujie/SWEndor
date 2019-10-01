using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Instances
{
  public class SurfaceVentATI : Groups.SurfaceTower
  {
    internal SurfaceVentATI(Factory owner) : base(owner, "VENT", "Thermal Exhaust Port")
    {
      MaxStrength = 12000;
      ImpactDamage = 120;

      RenderData.RadarSize = 2.5f;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Name, @"surface\surface_vent.x");
    }

    public override void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(engine, owner, hitby, impact, normal);

      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo is TorpedoATI) //hard code?
        owner.SetState_Dead();
    }
  }
}

