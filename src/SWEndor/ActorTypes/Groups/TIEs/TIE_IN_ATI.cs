using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class TIE_IN_ATI : Groups.TIE
  {
    internal TIE_IN_ATI(Factory owner) : base(owner, "TIEI", "TIE Interceptor")
    {
      SystemData.MaxShield = 0;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 55;

      ScoreData = new ScoreData(500, 1000);

      MeshData = new MeshData(Name, @"tie\tie_interceptor.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 5), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("TIEIWING", new TV_3DVECTOR(-30, 0, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("TIEIWING", new TV_3DVECTOR(30, 0, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        };

      Loadouts = new string[] { "TIEI_LASR", "TIEI_LASR_AI" };
    }
  }
}

