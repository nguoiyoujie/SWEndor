using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class CorellianATI : Groups.Warship
  {
    internal CorellianATI(Factory owner) : base(owner, "Corellian Corvette")
    {
      MaxStrength = 575.0f;
      ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 100.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 50.0f;
      MoveLimitData.MaxTurnRate = 9f;

      MoveLimitData.ZTilt = 5.5f;
      MoveLimitData.ZNormFrac = 0.015f;

      RenderData.CullDistance = 20000;
      Scale = 0.6f;

      ScoreData = new ScoreData(10, 5000);

      MeshData = new MeshData(Name, @"corellian\corellian.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 55, -35), new TV_3DVECTOR(0, 55, 2000)),
        new LookData(new TV_3DVECTOR(0, 300, -800), new TV_3DVECTOR(0, 0, 2000))
        };
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 500f, new TV_3DVECTOR(0, 0, -200), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("Corellian Turbolaser Tower", new TV_3DVECTOR(-35, 15, 110), new TV_3DVECTOR(-80, -90, 0), true)
        , new AddOnData("Corellian Turbolaser Tower", new TV_3DVECTOR(-35, 15, 80), new TV_3DVECTOR(-80, -90, 0), true)
        , new AddOnData("Corellian Turbolaser Tower", new TV_3DVECTOR(35, 15, 110), new TV_3DVECTOR(-80, 90, 0), true)
        , new AddOnData("Corellian Turbolaser Tower", new TV_3DVECTOR(35, 15, 80), new TV_3DVECTOR(-80, 90, 0), true)
        , new AddOnData("Corellian Turbolaser Tower", new TV_3DVECTOR(0, -45, 150), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("Corellian Turbolaser Tower", new TV_3DVECTOR(0, 45, 150), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }
  }
}

