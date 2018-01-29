using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Yavin4ATI : StaticSceneGroup
  {
    private static Yavin4ATI _instance;
    public static Yavin4ATI Instance()
    {
      if (_instance == null) { _instance = new Yavin4ATI(); }
      return _instance;
    }

    private Yavin4ATI() : base("Yavin4")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"yavin4.bmp";
        string texpath = Path.Combine(Globals.ImagePath, texname);
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
          int texA = Engine.Instance().TVTextureFactory.LoadAlphaTexture(texpath);
          Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        SourceMesh.CreateBox(50000, 50000, 0.001f);
        SourceMesh.SetTexture(Engine.Instance().TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


