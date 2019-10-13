using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_A_ATI : Groups.TIE
  {
    internal TIE_A_ATI(Factory owner) : base(owner, "TIEA", "TIE Advanced")
    {
      SystemData.MaxShield = 8;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 650;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 350;
      MoveLimitData.MaxTurnRate = 60;

      MoveLimitData.ZTilt = 1.75f;
      MoveLimitData.ZNormFrac = 0.005f;

      ScoreData = new ScoreData(800, 2000);

      RegenData = new RegenData(false, 0.15f, 0, 0, 0);

      MeshData = new MeshData(Name, @"tie\tie_advanced.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 5), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "TIEA_MISL", "TIEA_LASR" };
    }
  }
}

