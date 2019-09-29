using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class AcclamatorATI : Groups.StarDestroyer
  {
    internal AcclamatorATI(Factory owner) : base(owner, "Acclamator Assault Ship")
    {
      // Combat
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 1.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 650.0f;
      ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 110.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 8.0f;
      MoveLimitData.MaxTurnRate = 1.6f;

      RenderData.CullDistance = 30000;

      ScoreData = new ScoreData(50, 5000);
      Scale = 1.3f;


      MeshData = new MeshData(Name, @"acclamator\acclamator.x");

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 143, 65), new TV_3DVECTOR(0, 143, 2000)) };
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 500.0f, new TV_3DVECTOR(0, 0, -100), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        // Sides
        new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(101, 10, 460), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(150, 10, 360), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(198, 10, 260), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(-101, 10, 460), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(-150, 10, 360), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(-198, 10, 260), new TV_3DVECTOR(0, -72, 0), true)
        
        // Front
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(0, 10, 610), new TV_3DVECTOR(-90, 0, 0), true)

        // Top
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(145, 42, 70), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("Acclamator Turbolaser Tower", new TV_3DVECTOR(-145, 42, 70), new TV_3DVECTOR(-75, -90, 0), true)
      };
    }
  }
}

