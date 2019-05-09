using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YavinATI : Groups.StaticScene
  {
    internal YavinATI(Factory owner) : base(owner, "Yavin")
    {
      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);

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


