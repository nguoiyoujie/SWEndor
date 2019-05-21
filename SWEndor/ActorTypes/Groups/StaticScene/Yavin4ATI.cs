using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Yavin4ATI : Groups.StaticScene
  {
    internal Yavin4ATI(Factory owner) : base(owner, "Yavin4")
    {
      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);

        // 1 texture
        int tex = 0;
        string texname = @"yavin4.bmp";
        string alphatexname = @"yavin4alpha.bmp";
        string texpath = Path.Combine(Globals.ImagePath, "planets", texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, "planets", alphatexname);
        if (TrueVision.TVGlobals.GetTex(texname) == 0)
        {
          int texS = TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
          tex = TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        float size = 50000;
        SourceMesh.AddWall(tex, -size / 2, 0, size / 2, 0, size, -size / 2);
        //SourceMesh.CreateBox(50000, 50000, 0.001f);
        SourceMesh.SetTexture(tex);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


