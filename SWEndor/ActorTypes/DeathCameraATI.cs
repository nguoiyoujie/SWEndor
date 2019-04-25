using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
using System;

namespace SWEndor.ActorTypes
{
  public enum DeathCamMode { CIRCLE, FOLLOW };

  public class DeathCameraATI : ActorTypeInfo
  {
    internal DeathCameraATI(Factory owner) : base(owner, "Death Camera")
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

    public override void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerCameraInfo.Instance().Camera;

      float circleperiod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
      float angularphase = (Globals.Engine.Game.GameTime % circleperiod) * (2 * Globals.PI / circleperiod);
      float radius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
      float height = ainfo.CameraSystemInfo.CamDeathCircleHeight;

      TV_3DVECTOR pos = ainfo.GetPosition();
      switch (Globals.Engine.GameScenarioManager.Scenario.DeathCamMode)
      {
        case DeathCamMode.CIRCLE:
          cam.SetPosition(pos.x + radius * (float)Math.Cos(angularphase)
                        , pos.y + height
                        , pos.z + radius * (float)Math.Sin(angularphase));

          cam.SetLookAt(pos.x
                        , pos.y
                        , pos.z);
          break;
        case DeathCamMode.FOLLOW:
          TV_3DVECTOR pos2 = ainfo.GetRelativePositionFUR(0, ainfo.CameraSystemInfo.CamDeathCircleHeight, -ainfo.CameraSystemInfo.CamDeathCircleRadius, true);
          cam.SetPosition(pos2.x
                        , pos2.y
                        , pos2.z);

          cam.SetLookAt(pos.x
                        , pos.y
                        , pos.z);
          break;
      }
    }
  }
}


