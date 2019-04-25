using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class EndorATI : Groups.StaticScene
  {
    internal EndorATI(Factory owner) : base(owner, "Endor")
    {
      SourceMesh = Globals.Engine.TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Globals.Engine.TrueVision.TVScene.CreateMeshBuilder(Key);

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


