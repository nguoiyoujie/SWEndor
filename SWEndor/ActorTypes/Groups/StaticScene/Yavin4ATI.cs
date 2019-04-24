using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Yavin4ATI : Groups.StaticScene
  {
    private static Yavin4ATI _instance;
    public static Yavin4ATI Instance()
    {
      if (_instance == null) { _instance = new Yavin4ATI(); }
      return _instance;
    }

    private Yavin4ATI() : base("Yavin4")
    {
      SourceMesh = Globals.Engine.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Globals.Engine.TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = @"yavin4.bmp";
        string alphatexname = @"yavin4alpha.bmp";
        string texpath = Path.Combine(Globals.ImagePath, "planets", texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, "planets", alphatexname);
        if (Globals.Engine.TVGlobals.GetTex(texname) == 0)
        {
          int texS = Globals.Engine.TVTextureFactory.LoadTexture(texpath);
          int texA = Globals.Engine.TVTextureFactory.LoadTexture(alphatexpath);
          Globals.Engine.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        SourceMesh.CreateBox(50000, 50000, 0.001f);
        SourceMesh.SetTexture(Globals.Engine.TVGlobals.GetTex(texname));
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


