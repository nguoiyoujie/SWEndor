using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpL00 : Groups.Explosion
  {
    internal ExpL00(Factory owner) : base(owner, "ExpL00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RenderData.RadarSize = 5;
      RenderData.CullDistance = -1;
      MeshData = MeshDataDecorator.CreateBillboardAnimation(Name, 100, "explosion/large", ref texanimframes);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData("exp_resto", 500) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      PlayerCameraInfo.ProximityShake(25, 300, ainfo.Position);
    }
  }
}

