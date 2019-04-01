using System.IO;

namespace SWEndor.ActorTypes
{
  public class YavinATI : StaticSceneGroup
  {
    private static YavinATI _instance;
    public static YavinATI Instance()
    {
      if (_instance == null) { _instance = new YavinATI(); }
      return _instance;
    }

    private YavinATI() : base("Yavin")
    {
      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = Path.Combine("planets", @"yavin.bmp");
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


