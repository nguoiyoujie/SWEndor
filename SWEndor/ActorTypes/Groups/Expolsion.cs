using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
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
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CullDistance = 12500f;

      RadarSize = 1;

      IsExplosion = true;
    }

    protected void LoadAlphaTextureFromFolder(string mainPath, string subPath)
    {
      List<int> frames = new List<int>();
      string folderPath = Path.Combine(mainPath, subPath);
      foreach (string texpath in Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly))
      {
        string texname = Path.Combine(subPath, Path.GetFileName(texpath));
        frames.Add(LoadAlphaTexture(texname, texpath));
      }
      texanimframes = frames.ToArray();
    }

    protected void LoadTextureFromFolder(string mainPath, string subPath)
    {
      List<int> frames = new List<int>();
      string folderPath = Path.Combine(mainPath, subPath);
      foreach (string texpath in Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly))
      {
        string texname = Path.Combine(subPath, Path.GetFileName(texpath));
        frames.Add(LoadTexture(texname, texpath));
      }
      texanimframes = frames.ToArray();
    }


    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        TV_3DVECTOR pos = PlayerCameraInfo.Instance().Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
        ainfo.LookAtPoint(pos);

        int k = texanimframes.Length - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * texanimframes.Length);
        if (k >= 0 && k < texanimframes.Length)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}

