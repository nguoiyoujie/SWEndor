using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class SunATI : StaticSceneGroup
  {
    private static SunATI _instance;
    public static SunATI Instance()
    {
      if (_instance == null) { _instance = new SunATI(); }
      return _instance;
    }

    private SunATI() : base("Sun")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"sun.jpg";
        string texpath = Path.Combine(Globals.ImagePath, texname);
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
          int texA = Engine.Instance().TVTextureFactory.LoadAlphaTexture(texpath);
          Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        SourceMesh.CreateBox(10000, 10000, 0.001f);
        SourceMesh.SetTexture(Engine.Instance().TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


