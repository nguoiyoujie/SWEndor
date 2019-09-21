using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Groups
{
  public class Explosion : ActorTypeInfo
  {
    protected int atlasX = 1;
    protected int atlasY = 1;

    internal Explosion(Factory owner, string name): base(owner, name)
    {
      // Combat
      RenderData.CullDistance = 12500f;

      RenderData.RadarSize = 1;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_M;

      Mask = ComponentMask.EXPLOSION;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      TV_3DVECTOR pos = ainfo.GetEngine().PlayerCameraInfo.Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
      ainfo.LookAt(pos);

      int frames = atlasX * atlasY;
      int k = frames - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * frames);
      float su = 1f / atlasX;
      float sv = 1f / atlasY;
      float u = (k % atlasX) * su;
      float v = (k / atlasX) * sv;
      ainfo.SetTexMod(u, v, su, sv);
    }
  }
}

