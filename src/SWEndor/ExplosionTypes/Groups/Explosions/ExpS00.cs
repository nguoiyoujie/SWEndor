using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ExpS00 : Groups.Explosion
  {
    internal ExpS00(Factory owner) : base(owner, "EXPS00", "ExpS00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);
      ShakeData = new ShakeData(5, 100);

      RenderData.RadarSize = 2;

      ExplRenderData = new ExplRenderData(4, 2, 0.5f, 0);
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 10, "explosion/small/tex.jpg", MTV3D65.CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, ExplRenderData.AtlasX, ExplRenderData.AtlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpSm, 500) };
    }
  }
}

