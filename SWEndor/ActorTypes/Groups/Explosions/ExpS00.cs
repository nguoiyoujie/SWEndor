using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpS00 : Groups.Explosion
  {
    internal ExpS00(Factory owner) : base(owner, "ExpS00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RenderData.RadarSize = 2;

      atlasX = 4;
      atlasY = 2;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 10, "explosion/small/tex.jpg", atlasX, atlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpSm, 500) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.CyclePeriod = 0.5f;
      PlayerCameraInfo.ProximityShake(5, 100, ainfo.Position);
    }

  }
}

