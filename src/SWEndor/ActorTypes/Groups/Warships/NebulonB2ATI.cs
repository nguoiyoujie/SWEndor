using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class NebulonB2ATI : Groups.Warship
  {
    internal NebulonB2ATI(Factory owner) : base(owner, "NEB2", "Nebulon-B2 Frigate")
    {
      SystemData.MaxShield = 600;
      SystemData.MaxHull = 750;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 36.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 10.0f;
      MoveLimitData.MaxTurnRate = 2.5f;

      RenderData.CullDistance = 45000;
      ScoreData = new ScoreData(15, 10000);
      RegenData = new RegenData(false, 1.5f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"nebulonb\nebulonb2.x", 0.75f);
      DyingMoveData.Sink(0.02f, 5f, 0.8f);

      CameraData.Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 120, -300), new TV_3DVECTOR(0, 120, 2000)) };
      SoundData.SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500.0f, new TV_3DVECTOR(0, 100, -300), true, isEngineSound: true) };
      AddOnData.AddOns = new AddOnData[]
      {
         new AddOnData("NEBLLSR", new TV_3DVECTOR(0, 37, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(0, 80, -480), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(0, -140, -450), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("NEBLMPOD", new TV_3DVECTOR(-80, -45, -345), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("NEBLMPOD", new TV_3DVECTOR(80, -45, -345), new TV_3DVECTOR(0, 0, 0), true)

        , new AddOnData("NEBLLSR", new TV_3DVECTOR(240, -300, 225), new TV_3DVECTOR(45, 90, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(-240, -300, 225), new TV_3DVECTOR(-45, 90, 0), true)

        , new AddOnData("HANGAR", new TV_3DVECTOR(10, -24, 185), new TV_3DVECTOR(0, 180, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SpawnerInfo = SpawnerInfoDecorator.NebB_Default;
    }
  }
}

