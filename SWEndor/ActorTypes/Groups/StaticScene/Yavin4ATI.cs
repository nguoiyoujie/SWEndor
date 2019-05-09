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
        string texname = @"yavin4.bmp";
        string alphatexname = @"yavin4alpha.bmp";
        string texpath = Path.Combine(Globals.ImagePath, "planets", texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, "planets", alphatexname);
        if (TrueVision.TVGlobals.GetTex(texname) == 0)
        {
          int texS = TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
          TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        SourceMesh.CreateBox(50000, 50000, 0.001f);
        SourceMesh.SetTexture(TrueVision.TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


