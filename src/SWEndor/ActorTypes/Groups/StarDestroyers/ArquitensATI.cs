using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class ArquitensATI : Groups.StarDestroyer
  {
    internal ArquitensATI(Factory owner) : base(owner, "ARQT", "Arquitens Light Cruiser")
    {
      // Combat
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL01", 1, 1.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 150;
      SystemData.MaxHull = 300;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 125.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 8.0f;
      MoveLimitData.MaxTurnRate = 1.6f;

      RenderData.CullDistance = 35000;
      RegenData = new RegenData(false, 0.6f, 0, 0, 0);

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Engine, Name, @"arquitens\arquitens.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 80, -45), new TV_3DVECTOR(0, 80, 2000)),
        };
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 500.0f, new TV_3DVECTOR(0, 0, -180), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        // Sides
        new AddOnData("ARQTLSR", new TV_3DVECTOR(85, 22, 16), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("ARQTLSR", new TV_3DVECTOR(40, 22, 170), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("ARQTLSR", new TV_3DVECTOR(-85, 22, 16), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("ARQTLSR", new TV_3DVECTOR(-40, 22, 170), new TV_3DVECTOR(0, -72, 0), true)

        // Top
        , new AddOnData("ARQTLSR", new TV_3DVECTOR(38, 50, 16), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("ARQTLSR", new TV_3DVECTOR(-38, 50, 16), new TV_3DVECTOR(-75, -90, 0), true)
      };
    }
  }
}

