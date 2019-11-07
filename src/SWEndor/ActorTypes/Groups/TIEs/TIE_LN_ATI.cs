using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_LN_ATI : Groups.TIE
  {
    internal TIE_LN_ATI(Factory owner) : base(owner, "TIE", "TIE")
    {
      SystemData.MaxShield = 0;
      SystemData.MaxHull = 4;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 350;
      MoveLimitData.MinSpeed = 175;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 45;

      ScoreData = new ScoreData(400, 400);

      MeshData = new MeshData(Name, @"tie\tie.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 5), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("TIEWING", new TV_3DVECTOR(-30, 0, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("TIEWING", new TV_3DVECTOR(30, 0, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        };

      Loadouts = new string[] { "TIE__LASR", "TIE__LASR_AI" };
    }
  }
}

