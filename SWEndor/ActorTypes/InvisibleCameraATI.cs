namespace SWEndor.ActorTypes
{
  public class InvisibleCameraATI : ActorTypeInfo
  {
    internal InvisibleCameraATI(Factory owner) : base(owner, "Invisible Camera")
    {
      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


