using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class HothATI : Groups.StaticScene
  {
    private static HothATI _instance;
    public static HothATI Instance()
    {
      if (_instance == null) { _instance = new HothATI(); }
      return _instance;
    }

    private HothATI() : base("Hoth")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = Path.Combine("planets", @"hoth.bmp");
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int itex = LoadTexture(texname, texpath);

        SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, "planet", "endor.x"), true);
        SourceMesh.SetTexture(itex);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


