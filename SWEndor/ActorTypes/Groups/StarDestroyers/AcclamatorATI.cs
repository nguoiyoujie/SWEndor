using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
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
      MaxSpeed = 110.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 8.0f;
      MaxTurnRate = 1.6f;

      CullDistance = 30000;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"acclamator\acclamator.x");

      Cameras = new ActorCameraInfo[] { new ActorCameraInfo(new TV_3DVECTOR(0, 143, 65), new TV_3DVECTOR(0, 143, 2000)) };
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 500.0f, new TV_3DVECTOR(0, 0, -100), true) };
      AddOns = new AddOnInfo[]
      {
        // Sides
        new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(101, 10, 460), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(150, 10, 360), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(198, 10, 260), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-101, 10, 460), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-150, 10, 360), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-198, 10, 260), new TV_3DVECTOR(0, -72, 0), true)
        
        // Front
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(0, 10, 610), new TV_3DVECTOR(-90, 0, 0), true)

        // Top
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(145, 42, 70), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-145, 42, 70), new TV_3DVECTOR(-90, 0, 15), true)
      };
    }
  }
}

