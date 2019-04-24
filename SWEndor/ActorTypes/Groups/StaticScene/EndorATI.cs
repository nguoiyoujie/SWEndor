using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class EndorATI : Groups.StaticScene
  {
    private static EndorATI _instance;
    public static EndorATI Instance()
    {
      if (_instance == null) { _instance = new EndorATI(); }
      return _instance;
    }

    private EndorATI() : base("Endor")
    {
      SourceMesh = Globals.Engine.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Globals.Engine.TVScene.CreateMeshBuilder(Key);

        // 1 texture
        string texname = Path.Combine("planets", @"endor.jpg");
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


