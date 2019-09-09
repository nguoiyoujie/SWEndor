using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
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

      MeshData = MeshDataDecorator.CreateBillboardAnimation(Name, 1000, "explosion/large", ref texanimframes);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData("exp_nave", 3000) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      PlayerCameraInfo.ProximityShake(80, 2000, ainfo.Position);
    }
  }
}

