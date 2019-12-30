using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ExpL00 : Groups.Explosion
  {
    internal ExpL00(Factory owner) : base(owner, "EXPL00", "ExpL00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);
      ShakeData = new ShakeData(25, 300);

      RenderData.RadarSize = 5;
      RenderData.CullDistance = -1;

      ExplRenderData = new ExplRenderData(4, 4, 1, 0);
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Engine, Name, 100, "explosion/large/tex.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, ExplRenderData.AtlasX, ExplRenderData.AtlasY);

      SoundData.InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpMd, 1000) };
    }
  }
}

