using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class YavinATI : StaticSceneGroup
  {
    private static YavinATI _instance;
    public static YavinATI Instance()
    {
      if (_instance == null) { _instance = new YavinATI(); }
      return _instance;
    }

    private YavinATI() : base("Yavin")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"yavin.bmp";
        string texpath = Path.Combine(Globals.ImagePath, texname);
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          Engine.Instance().TVTextureFactory.LoadTexture(texpath, texname);
        }
        SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, @"endor.x"), true);
        SourceMesh.SetTexture(Engine.Instance().TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


