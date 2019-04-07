using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class ArquitensATI : StarDestroyerGroup
  {
    private static ArquitensATI _instance;
    public static ArquitensATI Instance()
    {
      if (_instance == null) { _instance = new ArquitensATI(); }
      return _instance;
    }

    private ArquitensATI() : base("Arquitens Light Cruiser")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 450.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 125.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 8.0f;
      MaxTurnRate = 1.6f;

      CullDistance = 20000;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"arquitens\arquitens.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 0, -180), 500.0f, true) };
      AddOns = new AddOnInfo[]
      {
        // Sides
        new AddOnInfo("Arquitens Turbolaser Tower", new TV_3DVECTOR(85, 22, 16), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Arquitens Turbolaser Tower", new TV_3DVECTOR(40, 22, 170), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Arquitens Turbolaser Tower", new TV_3DVECTOR(-85, 22, 16), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Arquitens Turbolaser Tower", new TV_3DVECTOR(-40, 22, 170), new TV_3DVECTOR(0, -72, 0), true)

        // Top
        , new AddOnInfo("Arquitens Turbolaser Tower", new TV_3DVECTOR(38, 50, 16), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Arquitens Turbolaser Tower", new TV_3DVECTOR(-38, 50, 16), new TV_3DVECTOR(-90, 0, 15), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      List<float[]> ttowers = new List<float[]>();

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 80, -45));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 80, 2000));

      ainfo.ExplosionInfo.DeathExplosionSize = 1.5f;
    }
  }
}

