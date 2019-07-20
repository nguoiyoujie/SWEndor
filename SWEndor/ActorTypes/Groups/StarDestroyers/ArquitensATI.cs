using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ArquitensATI : Groups.StarDestroyer
  {
    internal ArquitensATI(Factory owner) : base(owner, "Arquitens Light Cruiser")
    {
      // Combat
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 1.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

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

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 80, -45), new TV_3DVECTOR(0, 80, 2000)),
        };
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 500.0f, new TV_3DVECTOR(0, 0, -180), true) };
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
  }
}

