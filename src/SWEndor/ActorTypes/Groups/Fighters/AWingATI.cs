using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class AWingATI : Groups.RebelWing
  {
    internal AWingATI(Factory owner) : base(owner, "AWING", "A-Wing")
    {
      SystemData.MaxShield = 8;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 55;

      ScoreData = new ScoreData(500, 2500);

      RegenData = new RegenData(false, 0.16f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"awing\awing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 2, 20), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Loadouts = new WeapData[] 
      {
        new WeapData("TORP", "SEC_1_AI", "NO_AUTOAIM", "A_WG_TORP", "A_WG_TORP", "WING_TORP", "WING_TORP"),
        new WeapData("MISL", "SEC_1_AI", "NO_AUTOAIM", "A_WG_MISL", "A_WG_MISL", "WING_MISL", "WING_MISL"),
        new WeapData("LASR", "PRI_124_AI", "NO_AUTOAIM", "DEFAULT", "A_WG_LASR", "WING_LSR_R", "WING_LASER"),
      };
    }
  }
}

