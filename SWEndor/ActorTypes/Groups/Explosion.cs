using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Groups
{
  public class Explosion : ActorTypeInfo
  {
    protected int[] texanimframes = new int[0];

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
      if (!ainfo.IsDyingOrDead)
      {
        TV_3DVECTOR pos = ainfo.GetEngine().PlayerCameraInfo.Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
        ainfo.LookAt(pos);

        int k = texanimframes.Length - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * texanimframes.Length);
        if (k >= 0 && k < texanimframes.Length)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}

