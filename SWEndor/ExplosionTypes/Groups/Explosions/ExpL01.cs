using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ExpL01 : Groups.Explosion
  {
    internal ExpL01(Factory owner) : base(owner, "ExpL01")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RenderData.RadarSize = 20;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_L;
      RenderData.CullDistance = -1;

      atlasX = 4;
      atlasY = 4;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 1000, "explosion/large/tex.jpg", atlasX, atlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpLg, 3000) };
    }

    public override void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      base.Initialize(engine, ainfo);
      PlayerCameraInfo.ProximityShake(80, 2000, ainfo.Position);
    }
  }
}

