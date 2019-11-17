using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class WedgeXWingATI : Groups.RebelWing
  {
    internal WedgeXWingATI(Factory owner) : base(owner, "WEDGE", "X-Wing (Wedge)")
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      SystemData.MaxShield = 50;
      SystemData.MaxHull = 150;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 500;
      MoveLimitData.MaxTurnRate = 50;

      AIData.AggressiveTracker = true;
      ScoreData = new ScoreData(750, 10000);


      RenderData.AlwaysShowInRadar = true;

      RegenData = new RegenData(false, 0.2f, 0, 0, 0);

      MeshData = new MeshData(Name, @"xwing\xwing.x");
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineXWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
        };

      Loadouts = new string[] { "X_WG_TORP", "X_WG_LASR" };
    }
  }
}

