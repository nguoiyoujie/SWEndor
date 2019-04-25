using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Yavin4ATI : Groups.StaticScene
  {
    internal Yavin4ATI(Factory owner) : base(owner, "Yavin4")
    {
      SourceMesh = Globals.Engine.TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Globals.Engine.TrueVision.TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"yavin4.bmp";
        string alphatexname = @"yavin4alpha.bmp";
        string texpath = Path.Combine(Globals.ImagePath, "planets", texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, "planets", alphatexname);
        if (Globals.Engine.TrueVision.TVGlobals.GetTex(texname) == 0)
        {
          int texS = Globals.Engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = Globals.Engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
          Globals.Engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        SourceMesh.CreateBox(50000, 50000, 0.001f);
        SourceMesh.SetTexture(Globals.Engine.TrueVision.TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


