using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class DeathStarATI : StaticSceneGroup
  {
    private static DeathStarATI _instance;
    public static DeathStarATI Instance()
    {
      if (_instance == null) { _instance = new DeathStarATI(); }
      return _instance;
    }

    private DeathStarATI() : base("DeathStar")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"deathstar.bmp";
        string texpath = Path.Combine(Globals.ImagePath, texname);
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          //Engine.Instance().TVTextureFactory.LoadTexture(texpath, texname);
          int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
          int texA = Engine.Instance().TVTextureFactory.LoadAlphaTexture(texpath);
          Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        SourceMesh.CreateBox(20000, 20000, 0.001f);
        SourceMesh.SetTexture(Engine.Instance().TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


