using MTV3D65;
using System.IO;

namespace SWEndor.Game.Scene
{
  public class FlareInfo
  {
    public FlareInfo(TVTextureFactory texturefactory, string texpath)
    {
      TexID = texturefactory.LoadTexture(texpath, Path.GetFileNameWithoutExtension(texpath), -1, -1, CONST_TV_COLORKEY.TV_COLORKEY_NO, true);
    }

    public int TexID = -1;
    public float Size = 10;
    public float LinePosition = 40;
    public TV_COLOR NormalColor = new TV_COLOR(1, 1, 1, 0.5f);
    public TV_COLOR SpecularColor = new TV_COLOR(1, 1, 1, 0.5f);
  }
}
