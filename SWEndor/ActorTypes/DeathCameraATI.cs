using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class DeathCameraATI : ActorTypeInfo
  {
    private static DeathCameraATI _instance;
    public static DeathCameraATI Instance()
    {
      if (_instance == null) { _instance = new DeathCameraATI(); }
      return _instance;
    }

    private DeathCameraATI() : base("Death Camera")
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

    public override void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerInfo.Instance().Camera;

      float circleperiod = ainfo.CamDeathCirclePeriod;
      float angularphase = (Game.Instance().GameTime % circleperiod) * (2 * Globals.PI / circleperiod);
      float radius = ainfo.CamDeathCircleRadius;
      float height = ainfo.CamDeathCircleHeight;

      cam.SetPosition(ainfo.GetPosition().x + radius * (float)Math.Cos(angularphase)
                    , ainfo.GetPosition().y + height
                    , ainfo.GetPosition().z + radius * (float)Math.Sin(angularphase));

      cam.SetLookAt(ainfo.GetPosition().x
                    , ainfo.GetPosition().y
                    , ainfo.GetPosition().z);
    }
  }
}


