using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class SurfaceVentATI : Groups.SurfaceTower
  {
    internal SurfaceVentATI(Factory owner) : base(owner, "VENT", "Thermal Exhaust Port")
    {
      SystemData.MaxShield = 12000;
      CombatData.ImpactDamage = 120;

      RenderData.RadarSize = 2.5f;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Name, @"surface\surface_vent.x");
    }

    /*
    public override void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(engine, owner, hitby, impact, normal);

      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo is TorpedoATI) //hard code?
        owner.SetState_Dead();
    }
    */
  }
}

