namespace SWEndor.ActorTypes
{
  public class InvisibleCameraATI : ActorTypeInfo
  {
    internal InvisibleCameraATI(Factory owner) : base(owner, "Invisible Camera")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      EnableDistanceCull = false;
      CollisionEnabled = false;

      SourceMesh = Globals.Engine.TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Globals.Engine.TrueVision.TVScene.CreateMeshBuilder(Key);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


