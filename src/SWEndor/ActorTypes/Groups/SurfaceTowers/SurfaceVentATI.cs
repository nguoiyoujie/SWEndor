using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class SurfaceVentATI : Groups.SurfaceTower
  {
    internal SurfaceVentATI(Factory owner) : base(owner, "VENT", "Thermal Exhaust Port")
    {
      SystemData.MaxShield = 12000;
      CombatData.ImpactDamage = 120;

      RenderData.RadarSize = 2.5f;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Engine, Name, @"surface\surface_vent.x");
    }
  }
}

