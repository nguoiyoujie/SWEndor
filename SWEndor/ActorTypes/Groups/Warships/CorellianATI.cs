using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class CorellianATI : WarshipGroup
  {
    private static CorellianATI _instance;
    public static CorellianATI Instance()
    {
      if (_instance == null) { _instance = new CorellianATI(); }
      return _instance;
    }

    private CorellianATI() : base("Corellian Corvette")
    {
      MaxStrength = 575.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 100.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 50.0f;
      MaxTurnRate = 9f;

      ZTilt = 17.5f;
      ZNormFrac = 0.065f;

      Score_perStrength = 10;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"corellian\corellian.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 0, -200), 500.0f, true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(-35, 15, 110), new TV_3DVECTOR(-90, 0, 10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(-35, 15, 80), new TV_3DVECTOR(-90, 0, 10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(35, 15, 110), new TV_3DVECTOR(-90, 0, -10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(35, 15, 80), new TV_3DVECTOR(-90, 0, -10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(0, -45, 150), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(0, 45, 150), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 55, -35));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 55, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 300, -800));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));
    }
  }
}

