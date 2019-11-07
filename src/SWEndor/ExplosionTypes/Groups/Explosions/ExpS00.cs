using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ExpS00 : Groups.Explosion
  {
    internal ExpS00(Factory owner) : base(owner, "EXPS00", "ExpS00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RenderData.RadarSize = 2;

      atlasX = 4;
      atlasY = 2;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 10, "explosion/small/tex.jpg", MTV3D65.CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, atlasX, atlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpSm, 500) };
    }

    public override void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      base.Initialize(engine, ainfo);
      ainfo.CycleInfo.CyclePeriod = 0.5f;
      ActorInfo p = PlayerInfo.Actor;
      if (p != null && !p.IsDyingOrDead)
        PlayerCameraInfo.ProximityShake(5, 100, ainfo.Position);
    }

  }
}

