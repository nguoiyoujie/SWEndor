namespace SWEndor.Actors.Types
{
  public class InvisibleCameraATI : ActorTypeInfo
  {
    private static InvisibleCameraATI _instance;
    public static InvisibleCameraATI Instance()
    {
      if (_instance == null) { _instance = new InvisibleCameraATI(); }
      return _instance;
    }

    private InvisibleCameraATI() : base("Invisible Camera")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      EnableDistanceCull = false;
      CollisionEnabled = false;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}


