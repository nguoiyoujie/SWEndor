using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class EndorATI : StaticSceneGroup
  {
    private static EndorATI _instance;
    public static EndorATI Instance()
    {
      if (_instance == null) { _instance = new EndorATI(); }
      return _instance;
    }

    private EndorATI() : base("Endor")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"endor.jpg";
        string texpath = Path.Combine(Globals.ImagePath, texname);
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          Engine.Instance().TVTextureFactory.LoadTexture(texpath, texname);
        }
        SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, @"endor.x"), true);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


