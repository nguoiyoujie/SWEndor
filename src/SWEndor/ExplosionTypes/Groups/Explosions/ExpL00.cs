using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ExpL00 : Groups.Explosion
  {
    internal ExpL00(Factory owner) : base(owner, "EXPL00", "ExpL00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RenderData.RadarSize = 5;
      RenderData.CullDistance = -1;

      atlasX = 4;
      atlasY = 4;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 100, "explosion/large/tex.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, atlasX, atlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpMd, 1000) };
    }

    public override void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      base.Initialize(engine, ainfo);
      ActorInfo p = PlayerInfo.Actor;
      if (p != null && !p.IsDyingOrDead)
        PlayerCameraInfo.ProximityShake(25, 300, ainfo.Position);
    }
  }
}

