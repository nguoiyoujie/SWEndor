using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
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

    /*
    public override void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerInfo.Instance().Camera;
      TVMesh mesh = ainfo.Mesh;

      cam.ChaseCamera(mesh, new TV_3DVECTOR(0, 0, 0), new TV_3DVECTOR(0, 0, 2000), 100, true);
    }
    */
  }
}


