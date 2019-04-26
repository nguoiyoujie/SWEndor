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

      SourceMesh = FactoryOwner.Engine.TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = FactoryOwner.Engine.TrueVision.TVScene.CreateMeshBuilder(Key);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


