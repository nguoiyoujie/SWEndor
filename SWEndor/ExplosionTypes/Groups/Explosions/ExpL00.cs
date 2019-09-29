using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ExpL00 : Groups.Explosion
  {
    internal ExpL00(Factory owner) : base(owner, "ExpL00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RenderData.RadarSize = 5;
      RenderData.CullDistance = -1;

      atlasX = 4;
      atlasY = 4;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 100, "explosion/large/tex.jpg", atlasX, atlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpMd, 1000) };
    }

    public override void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      base.Initialize(engine, ainfo);
      PlayerCameraInfo.ProximityShake(25, 300, ainfo.Position);
    }
  }
}

