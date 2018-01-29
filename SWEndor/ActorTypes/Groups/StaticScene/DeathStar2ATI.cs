using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class DeathStar2ATI : StaticSceneGroup
  {
    private static DeathStar2ATI _instance;
    public static DeathStar2ATI Instance()
    {
      if (_instance == null) { _instance = new DeathStar2ATI(); }
      return _instance;
    }

    private DeathStar2ATI() : base("DeathStar2")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"deathstar2.bmp";
        string texpath = Path.Combine(Globals.ImagePath, texname);
        //string alphatexname = @"endor.jpg";
        //string alphatexpath = Path.Combine(Globals.ShaderPath, alphatexname);
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

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Sides
      float size = 20000;
      ainfo.SetStateS("AddOn_0", string.Format("Death Star Laser Source, {0}, {1}, {2}, 0, 0, 0, true", -0.13f * size, 0.2f * size, -0.04f * size));
    }
  }
}


